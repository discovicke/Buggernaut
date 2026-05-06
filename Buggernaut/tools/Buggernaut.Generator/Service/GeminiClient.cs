using System.Net;
using System.Text.Json;

namespace Buggernaut.Generator;

public class GeminiClient(string apiKey, int maxAttempts = 5)
{
    private static readonly HttpStatusCode[] RetryableStatusCodes =
    [
        HttpStatusCode.TooManyRequests, // 429 rate limit
        HttpStatusCode.ServiceUnavailable, // 503 overloaded / maintenance
        HttpStatusCode.BadGateway, // 502
        HttpStatusCode.GatewayTimeout, // 504
        HttpStatusCode.InternalServerError // 500 transient serverfel
    ];

    private readonly HttpClient _http = new();

    public async Task<string> GenerateAsync(string prompt)
    {
        var threeUrl =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";
        var twoFiveUrl =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

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

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(twoFiveUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(result);
                return document.RootElement
                    .GetProperty("candidates")
                    .EnumerateArray().First()
                    .GetProperty("content")
                    .GetProperty("parts").EnumerateArray().First()
                    .GetProperty("text").GetString() ?? "";
            }

            // Terminal errors, retry kommer inte hjälpa
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception($" 401 Unauthorized – kontrollera din API-nyckel.");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var body401 = await response.Content.ReadAsStringAsync();
                throw new Exception($" 400 Bad Request: {body401}");
            }

            if (!RetryableStatusCodes.Contains(response.StatusCode))
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($" HTTP {(int)response.StatusCode} – kan inte återförsöka: {errorBody}");
            }

            // Retryable error – lista ut hur länge du måste vänta och försök igen
            var waitSeconds = GetWaitSeconds(response, attempt);

            PrintRetryWarning((int)response.StatusCode, attempt, maxAttempts, waitSeconds);
            await WaitWithCountdown(waitSeconds);
        }

        throw new Exception($"Lyckades inte generera Gemini-respons efter {maxAttempts} försök.");
    }

    private static double GetWaitSeconds(HttpResponseMessage response, int attempt)
    {
        // Respect Retry-After header (value can be seconds or an HTTP-date)
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

    private static void PrintRetryWarning(int statusCode, int attempt, int maxAttempts, double waitSeconds)
    {
        var label = statusCode switch
        {
            429 => "Rate limit",
            503 => "Tjänsten otillgänglig",
            500 => "Serverfel",
            _   => $"HTTP {statusCode}"
        };

        Printer.Warn($"{label} ({statusCode})  –  försök {attempt}/{maxAttempts}  –  väntar {waitSeconds:F0} s", indent: 1);
    }

    private static async Task WaitWithCountdown(double totalSeconds)
    {
        var remaining = (int)Math.Ceiling(totalSeconds);
        while (remaining > 0)
        {
            Printer.Countdown(remaining);
            await Task.Delay(TimeSpan.FromSeconds(1));
            remaining--;
        }

        Printer.ClearLine();
    }
}