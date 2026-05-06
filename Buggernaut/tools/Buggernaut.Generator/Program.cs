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
        var prompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.BlackBox, Difficulties.Medium);
        var raw = await client.GenerateAsync(prompt);

        var challenge = ChallengeParser.Parse(raw);
        
        var scaffolder = new ExerciseScaffolder();
        scaffolder.Scaffold(challenge);
    }
}