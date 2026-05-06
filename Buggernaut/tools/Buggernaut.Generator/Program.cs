using Buggernaut.Generator;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        CliArgumentParser.GenerateOptions options;
        try
        {
            options = CliArgumentParser.Parse(args);
        }
        catch (CliArgumentException ex)
        {
            if (ex.IsError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n {ex.Message}");
                Console.ResetColor();
            }
            return;
        }

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
        var prompt = PromptBuilder.BuildUserPrompt(options.Category, options.Difficulty);

        const int maxValidationAttempts = 5;
        Challenge? challenge = null;

        for (int attempt = 1; attempt <= maxValidationAttempts; attempt++)
        {
            Console.WriteLine($"\nGenererar {options.Category} ({options.Difficulty})... (försök {attempt}/{maxValidationAttempts})");

            try
            {
                var raw = await client.GenerateAsync(prompt);
                var parsed = ChallengeParser.Parse(raw);
                var (isValid, reason) = ChallengeValidator.Validate(parsed);

                if (isValid)
                {
                    challenge = parsed;
                    break;
                }

                PrintValidationWarning(reason, attempt, maxValidationAttempts);
            }
            catch (Exception ex)
            {
                // HTTP-fel som inte kunde återhämtas, eller JSON-parsningsfel
                PrintExceptionWarning(ex.Message, attempt, maxValidationAttempts);

                // Avbryt omedelbart vid terminala fel (ex. 401 Unauthorized)
                if (ex.Message.Contains("401") || ex.Message.Contains("400"))
                    return;
            }
        }

        if (challenge is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n Kunde inte generera en giltig övning efter {maxValidationAttempts} försök.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n Övning genererad: \"{challenge.Title}\"");
        Console.ResetColor();

        var scaffolder = new ExerciseScaffolder();
        scaffolder.Scaffold(challenge);
    }

    private static void PrintValidationWarning(string reason, int attempt, int maxAttempts)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\t  Ogiltigt svar: {reason}");
        Console.ResetColor();

        if (attempt < maxAttempts)
            Console.WriteLine("\tBegär nytt svar från Gemini...");
    }

    private static void PrintExceptionWarning(string message, int attempt, int maxAttempts)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\t Fel: {message}");
        Console.ResetColor();

        if (attempt < maxAttempts)
            Console.WriteLine("\tFörsöker igen...");
    }
}