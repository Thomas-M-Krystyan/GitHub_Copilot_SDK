using GitHub.Copilot.SDK;

namespace Demo_English._2_Intermediate
{
    /// <summary>
    /// Connecting to MCP (Model Context Protocol) servers for extended tools.
    /// </summary>
    internal class F_McpServerIntegration
    {
        internal static async Task RequestAsync(string model, string prompt)
        {
            // Client
            await using var client = new CopilotClient();

            // MCP Servers
            var solutionDirectory =
                Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName // Default path for FilePath MCP to start from
                ?? Environment.CurrentDirectory;

            var mcpServers = new Dictionary<string, object>
            {
                // File System MCP Server
                ["filesystem"] = new Dictionary<string, object>
                {
                    ["command"] = "npx",
                    ["args"] = new[] { "-y", "@modelcontextprotocol/server-filesystem", solutionDirectory }
                }
            };

            // Session
            await using var session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model,
                Streaming = true,
                McpServers = mcpServers  // Add MCP server(s) into the session
            });
            
            // Listening to the streamed response
            session.On(sessionEvent =>
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
                    Console.WriteLine(new string('-', 50));  // Separator
                }
            });

            // Request
            _ = await session.SendAndWaitAsync(new MessageOptions 
            { 
                Prompt = prompt
            });

            // Interactive loop
            while (true)
            {
                // Prompt
                Console.Write("You: ");
                var input = Console.ReadLine();

                // Exit condition
                if (string.IsNullOrEmpty(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // Request
                Console.Write("Assistant: ");
                await session.SendAndWaitAsync(new MessageOptions { Prompt = input });
            }
        }
    }
}