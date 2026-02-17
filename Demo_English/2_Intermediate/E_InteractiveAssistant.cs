using GitHub.Copilot.SDK;
using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

// ReSharper disable NotAccessedPositionalProperty.Local

namespace Demo_English._2_Intermediate
{
    /// <summary>
    /// Interactive weather assistant with a conversation loop using a custom tool.
    /// </summary>
    internal class E_InteractiveAssistant
    {
        internal static async Task RequestAsync(string model, string? prompt = null)
        {
            // Client
            await using var client = new CopilotClient();

            // Tools (external public APIs)
            #region Real Weather API tool
            var getWeatherTool = AIFunctionFactory.Create(
                async Task<WeatherResult> ([Description("The city name")] string city) =>
                {
                    // HTTP client for weather API
                    using var httpClient = new HttpClient();

                    try
                    {
                        // Step 1: Get the geolocation (coordinates) of the city
                        var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1";
                        var geoResponse = await httpClient.GetFromJsonAsync<GeoResponse>(geoUrl);

                        if (geoResponse?.Results is null || geoResponse.Results.Length == 0)
                        {
                            // Handling the city that does not exist
                            return new WeatherResult(city, null, null, null, "City not found");
                        }

                        var location = geoResponse.Results[0];

                        // Step 2: Get weather data using previously obtained coordinates
                        var weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m,weather_code";
                        var weatherResponse = await httpClient.GetFromJsonAsync<WeatherResponse>(weatherUrl);

                        // Weather summary
                        var condition = GetWeatherCondition(weatherResponse?.Current?.WeatherCode ?? 0);
                        return new WeatherResult(
                            location.Name,
                            location.Country,
                            $"{weatherResponse?.Current?.Temperature}°C",
                            condition,
                            null
                        );
                    }
                    catch
                    {
                        // Issues with the Weather API
                        return new WeatherResult(city, null, null, null, "Failed to fetch weather data");
                    }

                    // URL EXAMPLE (weather in Madrid):
                    // https://api.open-meteo.com/v1/forecast?latitude=40.4165&longitude=-3.7026&current=temperature_2m,weather_code
                },
                "get_weather",
                "Get the current weather for a city"
            );
            #endregion

            #region Real Wikipedia City Fact tool
            var getCityFactTool = AIFunctionFactory.Create(
                async Task<CityFactResult> ([Description("The city name")] string city) =>
                {
                    using var httpClient = new HttpClient();
        
                    try
                    {
                        // Wikipedia REST API to get city summary
                        var wikiUrl = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(city)}";
                        httpClient.DefaultRequestHeaders.Add("User-Agent", "WeatherAssistant/1.0");
            
                        var response = await httpClient.GetFromJsonAsync<WikipediaSummary>(wikiUrl);
            
                        if (response?.Extract is null)
                        {
                            return new CityFactResult(city, "No fun facts found for this city.");
                        }
            
                        // Return the first 2-3 sentences as a "fun fact"
                        var fact = response.Extract;
                        return new CityFactResult(city, fact);
                    }
                    catch
                    {
                        return new CityFactResult(city, "Failed to fetch city information.");
                    }
                },
                "get_city_fact",
                "Get an interesting fact about a city"
            );
            #endregion

            // Session
            await using var session = await client.CreateSessionAsync(new SessionConfig
            {
                Model = model,
                Streaming = true,
                Tools = [getWeatherTool, getCityFactTool],  // Multiple tools
                SystemMessage = new SystemMessageConfig     // Integration of tools via system prompt
                {
                    Content = "You are a helpful weather assistant. Whenever a user asks about weather in a city," +
                              "always provide both the weather information and an interesting fun fact about that " +
                              "city using the available tools."
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

            // Welcome message
            Console.WriteLine("   Weather Assistant (type 'exit' to quit)");
            Console.WriteLine("   Try: 'What's the weather in Paris?' or 'Compare weather in New York and Los Angeles'\n");

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

        #region Weather API helpers and models
        // Helper method to convert weather code to condition string
        private static string GetWeatherCondition(int code) => code switch
        {
            0 => "clear sky",
            1 or 2 or 3 => "partly cloudy",
            45 or 48 => "foggy",
            51 or 53 or 55 => "drizzle",
            61 or 63 or 65 => "rainy",
            71 or 73 or 75 => "snowy",
            95 or 96 or 99 => "thunderstorm",
            _ => "unknown"
        };

        // DTO for weather tool result
        private record WeatherResult(string City, string? Country, string? Temperature, string? Condition, string? Error);

        // DTOs for API responses
        private record GeoResponse(GeoResult[]? Results);

        [UsedImplicitly]
        private record GeoResult(string Name, string Country, double Latitude, double Longitude);

        private record WeatherResponse(CurrentWeather? Current);

        [UsedImplicitly]
        private record CurrentWeather(
            [property: JsonPropertyName("temperature_2m")] double Temperature,
            [property: JsonPropertyName("weather_code")] int WeatherCode
        );
        #endregion

        #region Wikipedia API helpers and models
        // DTO for Wikipedia API response
        [UsedImplicitly]
        private record WikipediaSummary(
            string? Title,
            string? Extract,
            [property: JsonPropertyName("extract_html")] string? ExtractHtml
        );

        // DTO for city fact tool result
        private record CityFactResult(string City, string Fact);
        #endregion
    }
}