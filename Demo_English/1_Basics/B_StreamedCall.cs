using GitHub.Copilot.SDK;

namespace Demo_English._1_Basics
{
    /// <summary>
    /// The response will be streamed back as it is generated.
    /// </summary>
    internal class B_StreamedCall
    {
        internal static async Task RequestAsync(string model, string prompt)
        {
            // Client
            await using var client = new CopilotClient();

            // Session
            await using var session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model,
                Streaming = true
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
