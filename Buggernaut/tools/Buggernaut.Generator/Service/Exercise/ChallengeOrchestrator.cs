using Microsoft.Extensions.Configuration;

namespace Buggernaut.Generator;

internal static class ChallengeOrchestrator
{
    private const int MaxAttempts = 5;

    public static async Task RunAsync(CliArgumentParser.GenerateOptions options, IConfiguration config)
    {
        if (options.DryRun)
        {
            RunDryRun(options);
            return;
        }

        if (!ValidateApiKey(config)) return;

        Printer.H2($"Genererar  {options.Category}  ({options.Difficulty})");

        var client = LlmClientFactory.Create(config);
        var prompt = PromptBuilder.BuildUserPrompt(options.Category, options.Difficulty);
        var challenge = await TryGenerateChallengeAsync(client, prompt);

        if (challenge is null)
        {
            Printer.Blank();
            Printer.Error($"Kunde inte generera en giltig övning efter {MaxAttempts} försök.", indent: 1);
            return;
        }

        Printer.Info("Genererar filer...", indent: 1);
        new ExerciseScaffolder().Scaffold(challenge);
    }

    private static void RunDryRun(CliArgumentParser.GenerateOptions options)
    {
        Printer.H2("Dry-run");
        Printer.Info($"Genererar mock-övning  {options.Category}  ({options.Difficulty})");
        var mock = MockChallengeFactory.Create(options.Category, options.Difficulty);
        Printer.Ok($"Mock-övning klar: {mock.Title}");
        new ExerciseScaffolder().Scaffold(mock);
    }

    private static bool ValidateApiKey(IConfiguration config)
    {
        var provider = config["LLM:Provider"] ?? "Gemini";
        var apiKeyConfigPath = provider switch
        {
            "Gemini"    => "LLM:Gemini:ApiKey",
            "OpenAI"    => "LLM:OpenAI:ApiKey",
            "Anthropic" => "LLM:Anthropic:ApiKey",
            "Mistral"   => "LLM:Mistral:ApiKey",
            "Ollama"    => null,
            _           => null
        };

        if (apiKeyConfigPath != null && string.IsNullOrEmpty(config[apiKeyConfigPath]))
        {
            Printer.Error($"API-nyckel för {provider} saknas.");
            Printer.Dim("Kör en gång från tools/Buggernaut.Generator/:", indent: 1);
            Printer.Dim($"  dotnet user-secrets set \"{apiKeyConfigPath}\" \"din-nyckel\"", indent: 1);
            return false;
        }

        return true;
    }

    private static async Task<Challenge?> TryGenerateChallengeAsync(ILlmClient client, string prompt)
    {
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            Printer.Info($"Genererar utmaning  –  Försök {attempt}/{MaxAttempts}");

            try
            {
                var raw = await client.GenerateAsync(prompt);
                Printer.Info("Läser JSON-svar...", indent: 1);
                var parsed = ChallengeParser.Parse(raw);

                Printer.Info("Validerar JSON-svar...", indent: 1);
                var (isValid, reason) = ChallengeValidator.Validate(parsed);

                if (isValid) return parsed;

                Printer.Warn($"Ogiltigt svar: {reason}", indent: 1);
                if (attempt < MaxAttempts)
                    Printer.Dim("Begär nytt svar...", indent: 1);
            }
            catch (Exception ex)
            {
                Printer.Error(ex.Message, indent: 1);

                if (ex.Message.Contains("401") || ex.Message.Contains("400"))
                    return null;

                if (attempt < MaxAttempts)
                    Printer.Dim("Försöker igen...", indent: 1);
            }
        }

        return null;
    }
}

