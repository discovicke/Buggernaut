using Buggernaut.Generator;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var apiKey = config["Gemini:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API-nyckel saknas. Kör: dotnet user-secrets set \"Gemini:ApiKey\" \"din-nyckel\"");
            return;
        }

        var client = new GeminiClient(apiKey);
        var prompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.BlackBox, Difficulties.Hard);
        var raw = await client.GenerateAsync(prompt);

        Console.WriteLine("=== RAW ===");
        Console.WriteLine(raw);

        var challenge = ChallengeParser.Parse(raw);

        Console.WriteLine($"\n=== {challenge.Title} ===");
        Console.WriteLine(challenge.Description);
        Console.WriteLine("\n--- Buggy Code ---");
        Console.WriteLine(challenge.BuggyCode);
        Console.WriteLine("\n--- Hint ---");
        Console.WriteLine(challenge.Hint);
        Console.WriteLine("\n--- Solution Code ---");
        Console.WriteLine(challenge.SolutionCode);
        Console.WriteLine("\n--- Explanation ---");
        Console.WriteLine(challenge.Explanation);
        Console.WriteLine("\n--- Test Code ---");
        Console.WriteLine(challenge.TestCode);
    }
}