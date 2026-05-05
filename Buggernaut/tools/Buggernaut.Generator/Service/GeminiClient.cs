using System.Text.Json;

namespace Buggernaut.Generator;

public class GeminiClient(string apiKey)
{
    private readonly HttpClient _http = new();

    public async Task<string> GenerateAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var body = new
        {
            system_instruction = new
            {
                parts = new[] { new { text = PromptBuilder.SystemPrompt } }
            },
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };
        
        var json = JsonSerializer.Serialize(body);

        for (int attempt = 0; attempt < 3; attempt++)
        {
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(url, content);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Rate limit, väntar 10 sekunder...");
                Console.ResetColor();
                await Task.Delay(TimeSpan.FromSeconds(10));
                continue;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
        
            using var document = JsonDocument.Parse(result);
            return document.RootElement
                .GetProperty("candidates")
                .EnumerateArray().First()
                .GetProperty("content")
                .GetProperty("parts").EnumerateArray().First()
                .GetProperty("text").GetString() ?? "";
        }

        throw new Exception("Lyckades inte generera Gemini respons efter 3 försök.");
    }
}