using GitHub.Copilot.SDK;

namespace Demo_English._1_Basics
{
    /// <summary>
    /// You have to wait for the full response to be generated before you can access it.
    /// </summary>
    internal class A_BasicCall
    {
        internal static async Task RequestAsync(string model, string prompt)
        {
            // Client
            await using var client = new CopilotClient();

            // Session
            await using var session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model
            });

            // Request
            var response = await session.SendAndWaitAsync(new MessageOptions
            {
                Prompt = prompt
            });

            // Response
            Console.WriteLine(response?.Data.Content);
            Console.WriteLine(new string('-', 50));  // Separator
        }
    }
}
