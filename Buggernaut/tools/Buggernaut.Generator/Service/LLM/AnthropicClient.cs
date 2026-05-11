using System.Net;
using System.Text;
using System.Text.Json;

namespace Buggernaut.Generator.Service.LLM_Clients;

public class AnthropicClient(string apiKey, string model = "claude-3-5-haiku-latest", int maxAttempts = 5) : ILlmClient
{
    private static readonly HttpStatusCode[] RetryableStatusCodes =
    [
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.BadGateway,
        HttpStatusCode.GatewayTimeout,
        HttpStatusCode.InternalServerError
    ];

    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://api.anthropic.com/v1/")
    };

    public async Task<string> GenerateAsync(string prompt)
    {
        _http.DefaultRequestHeaders.Remove("x-api-key");
        _http.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _http.DefaultRequestHeaders.Remove("anthropic-version");
        _http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var body = new
        {
            model,
            max_tokens = 8192,
            system = PromptBuilder.SystemPrompt,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(body);

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            var apiTail = $"Anropar {model}  –  försök {attempt}/{maxAttempts}";
            using (var apiSpinner = new Spinner("Väntar på API", apiTail))
            {
                response = await _http.PostAsync("messages", content);
                apiSpinner.Stop(success: response.IsSuccessStatusCode);
            }
            Console.WriteLine();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("401 Unauthorized – kontrollera din API-nyckel.");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"400 Bad Request: {errorBody}");
            }

            if (!RetryableStatusCodes.Contains(response.StatusCode))
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP {(int)response.StatusCode} – kan inte återförsöka: {errorBody}");
            }

            var waitSeconds = GetWaitSeconds(response, attempt);
            var label = response.StatusCode switch
            {
                HttpStatusCode.TooManyRequests => "Rate limit",
                HttpStatusCode.ServiceUnavailable => "Tjänsten otillgänglig",
                HttpStatusCode.InternalServerError => "Serverfel",
                _ => $"HTTP {(int)response.StatusCode}"
            };

            var retryMessage =
                $"{label} ({(int)response.StatusCode})  –  försök {attempt}/{maxAttempts}  –  väntar {waitSeconds:F0} s";

            await WaitWithSpinner("Väntar på API", retryMessage, waitSeconds);
            Console.WriteLine();
        }

        throw new Exception($"Lyckades inte generera Anthropic-respons efter {maxAttempts} försök.");
    }

    private static double GetWaitSeconds(HttpResponseMessage response, int attempt)
    {
        if (response.Headers.RetryAfter is { } retryAfter)
        {
            if (retryAfter.Delta is { } delta)
                return Math.Max(delta.TotalSeconds, 1);

            if (retryAfter.Date is { } date)
            {
                var diff = date - DateTimeOffset.UtcNow;
                if (diff.TotalSeconds > 0)
                    return diff.TotalSeconds;
            }
        }

        var baseDelay = Math.Min(5 * Math.Pow(2, attempt - 1), 120);
        var jitter = Random.Shared.NextDouble() * 2;
        return baseDelay + jitter;
    }

    private static async Task WaitWithSpinner(string spinnerText, string warnText, double totalSeconds)
    {
        var remaining = (int)Math.Ceiling(totalSeconds);
        using var spinner = new Spinner(spinnerText, warnText);
        while (remaining > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            remaining--;
        }
        spinner.Stop(success: false);
    }
}

