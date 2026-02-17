using GitHub.Copilot.SDK;

namespace Demo_English._2_Intermediate
{
    /// <summary>
    /// Multi-agent collaboration using different LLM models working together.
    /// Demonstrates how multiple agents with different reasoning profiles and
    /// different expertise can collaborate on a complex task.
    /// </summary>
    internal class G_MultiAgent
    {
        internal static async Task RequestAsync(string userTask)
        {
            // Client
            await using var client = new CopilotClient();

            Console.WriteLine(".----------------------------------------------------------------.");
            Console.WriteLine("|         Multi-Agent Collaboration System                       |");
            Console.WriteLine("'----------------------------------------------------------------'\n");
            Console.WriteLine($"Task: {userTask}\n");
            Console.WriteLine(new string('=', 64));

            #region Agent 1: Creative Ideation Agent (GPT-4o)
            // Session for Agent 1
            await using var creativeAgent = await client.CreateSessionAsync(new SessionConfig
            {
                Model = "gpt-4o",  // NOTE: Better in creative tasks, producing ideas, metaphors, etc. Feels more "alive"
                Streaming = true,
                SystemMessage = new SystemMessageConfig
                {
                    Content = """
                        You are a Creative Ideation Specialist focused on innovative thinking and brainstorming.
                        Your role is to generate creative, out-of-the-box ideas and approaches.
                        Be imaginative, consider multiple perspectives, and suggest novel solutions.
                        IMPORTANT: Provide EXACTLY 3 ideas in bullet points. No introduction, no conclusion. Just 3 bullets.
                        """
                }
            });

            // Listening to the streamed response
            creativeAgent.On(sessionEvent =>
            {
                // "AssistantMessageDelta" is a partial chunk of an assistant's response, arriving in real-time as it's generated
                if (sessionEvent is AssistantMessageDeltaEvent deltaEvent)
                {
                    // Response (chunk)
                    Console.Write(deltaEvent.Data.DeltaContent);
                }

                // "SessionIdleEvent" indicates that the session is idle, meaning there are no ongoing operations
                if (sessionEvent is SessionIdleEvent)
                {
                    // Response (end line)
                    Console.WriteLine();
                    Console.WriteLine(new string('-', 64));  // Separator
                }
            });
            #endregion

            #region Agent 2: Technical Architect Agent (Claude Sonnet 4.5)
            // Session for Agent 2
            await using var architectAgent = await client.CreateSessionAsync(new SessionConfig
            {
                Model = "claude-sonnet-4.5",  // NOTE: Strong in technical analysis (e.g., architecture or programming)
                Streaming = true,
                SystemMessage = new SystemMessageConfig
                {
                    Content = """
                        You are a Senior Technical Architect with expertise in software design and best practices.
                        Your role is to evaluate ideas from a technical feasibility and architecture perspective.
                        Focus on scalability, maintainability, security, and practical implementation.
                        IMPORTANT: Provide EXACTLY 3 technical points in bullet format. No introduction, no conclusion.
                        """
                }
            });

            // Listening to the streamed response
            architectAgent.On(sessionEvent =>
            {
                // "AssistantMessageDelta" is a partial chunk of an assistant's response, arriving in real-time as it's generated
                if (sessionEvent is AssistantMessageDeltaEvent deltaEvent)
                {
                    // Response (chunk)
                    Console.Write(deltaEvent.Data.DeltaContent);
                }

                // "SessionIdleEvent" indicates that the session is idle, meaning there are no ongoing operations
                if (sessionEvent is SessionIdleEvent)
                {
                    // Response (end line)
                    Console.WriteLine();
                    Console.WriteLine(new string('-', 64));  // Separator
                }
            });
            #endregion

            #region Agent 3: Synthesis Agent (GPT-4.1)
            // Session for Agent 3
            await using var synthesisAgent = await client.CreateSessionAsync(new SessionConfig
            {
                Model = "gpt-4.1",  // NOTE: General purpose LLM, excellent at synthesis and balanced recommendations
                Streaming = true,
                SystemMessage = new SystemMessageConfig
                {
                    Content = """
                        You are a Strategic Synthesis Specialist who combines diverse inputs into coherent recommendations.
                        Your role is to merge creative ideas with technical constraints into actionable plans.
                        Provide a balanced, structured final recommendation with clear next steps.
                        IMPORTANT: Provide EXACTLY 4 actionable recommendations in bullet format. No introduction, no conclusion.
                        """
                }
            });

            // Listening to the streamed response
            var finalRecommendation = string.Empty;
            synthesisAgent.On(sessionEvent =>
            {
                // "AssistantMessageDelta" is a partial chunk of an assistant's response, arriving in real-time as it's generated
                if (sessionEvent is AssistantMessageDeltaEvent deltaEvent)
                {
                    // Response (chunk)
                    Console.Write(deltaEvent.Data.DeltaContent);
                    finalRecommendation += deltaEvent.Data.DeltaContent;  // Collecting the full response
                }

                // "SessionIdleEvent" indicates that the session is idle, meaning there are no ongoing operations
                if (sessionEvent is SessionIdleEvent)
                {
                    // Response (end line)
                    Console.WriteLine();
                    Console.WriteLine(new string('=', 64));  // Separator
                }
            });
            #endregion

            #region Agent 4: Documentation Agent with MCP Server
            // MCP Servers configuration
            var repositoryRoot =
                Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName  // Default path for FilePath MCP to start from
                ?? Environment.CurrentDirectory;

            var mcpServers = new Dictionary<string, object>
            {
                // File System MCP Server
                ["filesystem"] = new Dictionary<string, object>
                {
                    ["command"] = "npx",
                    ["args"] = new[] { "-y", "@modelcontextprotocol/server-filesystem", repositoryRoot }
                }
            };

            // Session for Agent 4
            await using var documentationAgent = await client.CreateSessionAsync(new SessionConfig
            {
                Model = "gpt-4o",  // NOTE: Good at formatting and structured documentation
                Streaming = true,
                McpServers = mcpServers,  // Add MCP server(s) into the session
                SystemMessage = new SystemMessageConfig
                {
                    Content = """
                              You are a Documentation Specialist who creates well-structured markdown documentation.
                              Your role is to format recommendations into clear, professional markdown document.
                              Use proper markdown formatting with headers, lists, code blocks, and emphasis where appropriate.
                              """
                }
            });

            // Listening to the streamed response
            documentationAgent.On(sessionEvent =>
            {
                // "AssistantMessageDelta" is a partial chunk of an assistant's response, arriving in real-time as it's generated
                if (sessionEvent is AssistantMessageDeltaEvent deltaEvent)
                {
                    // Response (chunk)
                    Console.Write(deltaEvent.Data.DeltaContent);
                }

                // "SessionIdleEvent" indicates that the session is idle, meaning there are no ongoing operations
                if (sessionEvent is SessionIdleEvent)
                {
                    // Response (end line)
                    Console.WriteLine();
                    Console.WriteLine(new string('-', 64));  // Separator
                }
            });
            #endregion

            #region Orchestration
            // Step 1: Creative Agent generates ideas
            Console.WriteLine("\n[Agent 1: Creative Thinking - GPT-4o]");
            Console.WriteLine(new string('-', 64));

            var creativeResponse = await creativeAgent.SendAndWaitAsync(
                new MessageOptions
                {
                    Prompt = $"Generate 3 creative ideas for: {userTask}"
                },
                cancellationToken: new CancellationTokenSource(TimeSpan.FromMinutes(3)).Token
            );
            var creativeIdeas = creativeResponse?.Data.Content ?? "No response";

            // Step 2: Technical Architect evaluates and refines
            Console.WriteLine("\n[Agent 2: Technical Architect - Claude Sonnet 4.5]");
            Console.WriteLine(new string('-', 64));

            var architectPrompt = $"""
                Task: {userTask}
                Ideas: {creativeIdeas}
                
                Provide 3 technical evaluation points.
                """;

            var architectResponse = await architectAgent.SendAndWaitAsync(
                new MessageOptions
                {
                    Prompt = architectPrompt
                },
                cancellationToken: new CancellationTokenSource(TimeSpan.FromMinutes(3)).Token
            );
            var technicalAnalysis = architectResponse?.Data.Content ?? "No response";

            // Step 3: Synthesis Agent creates final recommendation
            Console.WriteLine("\n[Agent 3: Strategic Synthesis - GPT-4.1]");
            Console.WriteLine(new string('-', 64));

            var synthesisPrompt = $"""
                Task: {userTask}
                Ideas: {creativeIdeas}
                Analysis: {technicalAnalysis}
                
                Provide 4 actionable recommendations.
                """;

            await synthesisAgent.SendAndWaitAsync(
                new MessageOptions
                {
                    Prompt = synthesisPrompt
                },
                cancellationToken: new CancellationTokenSource(TimeSpan.FromMinutes(3)).Token
            );

            // Step 4: Documentation Agent writes to markdown file
            Console.WriteLine("\n[Agent 4: Documentation with MCP]");
            Console.WriteLine(new string('-', 64));

            var documentationPrompt = $"""
                Save to "GitHub_Copilot_SDK/Recommendation.md":
                
                # Multi-Agent Collaboration Results
                
                ## Task
                {userTask}
                
                ## Creative Ideas
                {creativeIdeas}
                
                ## Technical Analysis
                {technicalAnalysis}
                
                ## Final Recommendations
                {finalRecommendation}
                
                Use the filesystem tool to write this markdown file. Confirm when done.
                """;

            await documentationAgent.SendAndWaitAsync(
                new MessageOptions
                {
                    Prompt = documentationPrompt
                },
                cancellationToken: new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token
            );
            #endregion

            Console.WriteLine("\n[OK] Multi-agent collaboration complete!");
            Console.WriteLine("  - Creative Agent (GPT-4o) contributed ideas");
            Console.WriteLine("  - Architect Agent (Claude Sonnet 4.5) evaluated feasibility");
            Console.WriteLine("  - Synthesis Agent (GPT-4.1) created final recommendation");
            Console.WriteLine("  - Documentation Agent with MCP wrote results to a Markdown file\n");
        }
    }
}