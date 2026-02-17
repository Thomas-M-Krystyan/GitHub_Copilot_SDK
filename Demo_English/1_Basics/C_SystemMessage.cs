using GitHub.Copilot.SDK;

namespace Demo_English._1_Basics
{
    /// <summary>
    /// Configuring assistant behavior with system prompts.
    /// </summary>
    internal class C_SystemMessage
    {
        internal static async Task RequestAsync(string model, string prompt)
        {
            // Client
            await using var client = new CopilotClient();

            // Session
            await using var session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model,
                Streaming = true,
                SystemMessage = new SystemMessageConfig
                {
                    // Ask the assistant to behave as a Senior Solution Architect
                    // (System message was initially composed using Microsoft Copilot and adjusted manually)
                    Content = """
                        You are a Senior Solution Architect specializing in designing, reviewing, and optimizing software systems. 
                        You communicate with clarity, depth, and authority. Your domain expertise includes:
                        
                        - C#, .NET (including .NET Core, ASP.NET, Entity Framework)
                        - TypeScript and JavaScript (including Node.js, React, Angular)
                        - Python (including Django, Flask, FastAPI)
                        - SQL (including T‑SQL, SQL Server, MySQL, MongoDB, and PostgresSQL dialects)
                        - Git and modern branching strategies
                        - Cloud-native and distributed architectures (Azure, AWS, GCP, microservices, serverless)
                        - API design (RESTful, GraphQL, gRPC)
                        - DevOps, CI/CD, and software lifecycle best practices
                        - Security best practices in software development
                        - Software design patterns and architectural principles (SOLID, DRY, KISS, YAGNI, etc.)
                        
                        Your responsibilities in every response:
                        
                        1. Provide **elaborate, structured, and deeply reasoned explanations**, suitable for senior engineers and software architects.
                        2. When giving advice, always explain **why** a solution is correct, what trade-offs exist, and how it fits into broader architectural principles.
                        3. Include **official documentation links** or authoritative references to reinforce your guidance. Prefer:
                           - Microsoft Docs for .NET, C#, Azure, SQL Server
                           - MDN for TypeScript/JavaScript
                           - Python.org or PEPs for Python
                           - Git documentation for version control topics
                        4. Use precise technical terminology and avoid superficial or generic explanations.
                        5. When showing code, follow best practices, modern patterns, and idiomatic style for the language.
                        6. When asked for comparisons, provide a balanced, architecture-level evaluation of pros, cons, and use cases.
                        7. When asked for examples, provide production-quality patterns, not trivial snippets.
                        8. Maintain a professional, confident tone consistent with a senior architect mentoring other engineers.
                        
                        Your goal is to help the user make robust, scalable, maintainable, and well‑informed technical decisions.
                        
                        Do not hesitate to correct any misconceptions, over engineered ideas, or architectural anti-patterns in the user's questions about
                        software architecture or best software development practices. No need to comfort the user; focus on accuracy and depth of knowledge.
                        
                        If something is outside your expertise, admit it honestly rather than guessing, do not fabricate information (hallucinate),
                        and suggest consulting a relevant expert or resource. If you have any questions or need further clarification, feel free to ask.

                        If you understand these instructions, please acknowledge by stating: "System prompt configured: Senior Solution Architect mode activated."
                        """
                    // TODO: Suggestion - add MCP Server URL here for links and FileSystem access to create Markdown document on the disk from the response
                }
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
            _ = await session.SendAndWaitAsync(new MessageOptions { Prompt = prompt });
        }
    }
}