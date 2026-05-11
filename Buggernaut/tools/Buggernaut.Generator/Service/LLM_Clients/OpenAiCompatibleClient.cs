using System.Net;
using System.Text;
using System.Text.Json;

namespace Buggernaut.Generator.Service.LLM_Clients;

public class OpenAiCompatibleClient : ILlmClient
{
    private static readonly HttpStatusCode[] RetryableStatusCodes =
    [
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.BadGateway,
        HttpStatusCode.GatewayTimeout,
        HttpStatusCode.InternalServerError
    ];

    private readonly HttpClient _http;
    private readonly string _model;
    private readonly string _apiKey;
    private readonly int _maxAttempts;

    public OpenAiCompatibleClient(string baseUrl, string apiKey, string model, int maxAttempts = 5)
    {
        var normalizedBase = baseUrl.TrimEnd('/') + '/';
        _http = new HttpClient { BaseAddress = new Uri(normalizedBase) };
        _apiKey = apiKey;
        _model = model;
        _maxAttempts = maxAttempts;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var body = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = PromptBuilder.SystemPrompt },
                new { role = "user",   content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(body);

        for (int attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            var apiTail = $"Anropar {_model}  –  försök {attempt}/{_maxAttempts}";
            using (var apiSpinner = new Spinner("Väntar på API", apiTail))
            {
                response = await _http.PostAsync("chat/completions", content);
                apiSpinner.Stop(success: response.IsSuccessStatusCode);
            }
            Console.WriteLine();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
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
                $"{label} ({(int)response.StatusCode})  –  försök {attempt}/{_maxAttempts}  –  väntar {waitSeconds:F0} s";

            await WaitWithSpinner("Väntar på API", retryMessage, waitSeconds);
            Console.WriteLine();
        }

        throw new Exception($"Lyckades inte generera svar efter {_maxAttempts} försök.");
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