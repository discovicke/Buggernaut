using System.Text.Json;

namespace Buggernaut.Generator;

public class ChallengeParser
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    
    public static Challenge Parse(string json)
    {
        // Ibland genererar Gemini markdown trots instruktioner
        var clean = json
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();
        
        return JsonSerializer.Deserialize<Challenge>(json)
               ?? throw new Exception("Kunde inte parsea Geminis svar till en utmaning.");
    }
}