using GitHub.Copilot.SDK;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace Demo_English._1_Basics
{
    /// <summary>
    /// Adding and using a custom tool to be called by the assistant.
    /// </summary>
    internal class D_UsingTools
    {
        internal static async Task RequestAsync(string model, string prompt)
        {
            // Client
            await using var client = new CopilotClient();

            // Tool (internal hardcoded function)
            #region Simple tool implementation
            var convertCurrencyTool = AIFunctionFactory.Create(
                ([Description("Amount to convert")] decimal amount,
                    [Description("Source currency code (e.g., USD)")] string from,
                    [Description("Target currency code (e.g., EUR)")] string to) =>
                {
                    // Mocked exchange rates
                    var rates = new Dictionary<string, decimal>
                    {
                        // Euro
                        ["EUR_USD"] = 1.09m,
                        ["EUR_GBP"] = 0.86m,
                        ["EUR_CHF"] = 0.98m,

                        // US Dollar
                        ["USD_EUR"] = 0.92m,
                        ["USD_GBP"] = 0.79m,
                        ["USD_CHF"] = 0.91m,

                        // British Pound
                        ["GBP_EUR"] = 1.16m,
                        ["GBP_USD"] = 1.27m,
                        ["GBP_CHF"] = 1.15m,

                        // Swiss Franc
                        ["CHF_EUR"] = 1.02m,
                        ["CHF_USD"] = 1.10m,
                        ["CHF_GBP"] = 0.87m
                    };
                    
                    var key = $"{from}_{to}";
                    var rate = rates.GetValueOrDefault(key, 0m);

                    // Validation for unsupported currency pairs
                    if (rate == 0m)
                    {
                        Console.WriteLine($"Unsupported currency conversion requested: {from} to {to}");
                        return new { amount, from, to, convertedAmount = 0m, exchangeRate = 0m };
                    }

                    var converted = amount * rate;
        
                    return new { amount, from, to, convertedAmount = converted, exchangeRate = rate };
                },
                "convert_currency",
                "Convert an amount from one currency to another"
            );
            #endregion

            // Session
            await using var session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model,
                Streaming = true,
                Tools = [convertCurrencyTool]  // Add the tool(s) to the session
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
