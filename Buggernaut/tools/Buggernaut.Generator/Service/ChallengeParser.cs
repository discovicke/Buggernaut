using System.Text.Json;

namespace Buggernaut.Generator;

public class ChallengeParser
{
    public static Challenge Parse(string json)
    {
        return JsonSerializer.Deserialize<Challenge>(json);
    }
}