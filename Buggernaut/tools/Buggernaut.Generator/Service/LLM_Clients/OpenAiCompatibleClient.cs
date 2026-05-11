using System.Text;
using System.Text.Json;
using Buggernaut.Generator;

public class OpenAiCompatibleClient : ILlmClient
{
    private readonly HttpClient _http;
    private readonly string _model;
    private readonly string _apiKey;

    public OpenAiCompatibleClient(string baseUrl, string apiKey, string model)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<string> GenerateAsync(string systemPrompt, string userPrompt)
    {
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var body = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userPrompt }
            }
        };

        var response = await _http.PostAsync("/v1/chat/completions",
            new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
    }
}