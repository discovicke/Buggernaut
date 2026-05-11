using System.Text.Json;

namespace Buggernaut.Generator;

public static class ChallengeParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static Challenge Parse(string json)
    {
        var clean = json
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        return JsonSerializer.Deserialize<Challenge>(clean, Options)
               ?? throw new Exception("Kunde inte parsea Geminis svar till en utmaning.");
    }
}