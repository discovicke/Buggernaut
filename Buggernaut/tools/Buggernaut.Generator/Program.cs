using Buggernaut.Generator;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        Printer.H1("Buggernaut");

        CliArgumentParser.GenerateOptions options;
        try
        {
            options = CliArgumentParser.Parse(args);
        }
        catch (CliArgumentException ex)
        {
            if (ex.IsError)
                Printer.Error(ex.Message);
            return;
        }
        
        if (options.Command == Command.Hint)
        {
            MetaReader.ShowHint(options.TargetClassName);
            return;
        }

        if (options.Command == Command.Explain)
        {
            MetaReader.ShowExplanation(options.TargetClassName);
            return;
        }

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<Program>()
            .Build();

        if (options.DryRun)
        {
            Printer.H2("Dry-run");
            Printer.Info($"Genererar mock-övning  {options.Category}  ({options.Difficulty})");

            var mock = MockChallengeFactory.Create(options.Category, options.Difficulty);
            Printer.Ok($"Mock-övning klar: {mock.Title}");

            var dryScaffolder = new ExerciseScaffolder();
            dryScaffolder.Scaffold(mock);
            return;
        }

        var provider = config["LLM:Provider"] ?? "Gemini";
        var apiKeyConfigPath = provider switch
        {
            "Gemini"    => "LLM:Gemini:ApiKey",
            "OpenAI"    => "LLM:OpenAI:ApiKey",
            "Anthropic" => "LLM:Anthropic:ApiKey",
            "Mistral"   => "LLM:Mistral:ApiKey",
            "Ollama"    => null, // Ollama kräver ingen API-nyckel
            _           => null
        };

        if (apiKeyConfigPath != null && string.IsNullOrEmpty(config[apiKeyConfigPath]))
        {
            Printer.Error($"API-nyckel för {provider} saknas.");
            Printer.Dim($"Kör en gång från tools/Buggernaut.Generator/:", indent: 1);
            Printer.Dim($"  dotnet user-secrets set \"{apiKeyConfigPath}\" \"din-nyckel\"", indent: 1);
            return;
        }

        Printer.H2($"Genererar  {options.Category}  ({options.Difficulty})");

        var client = LlmClientFactory.Create(config);
        var prompt = PromptBuilder.BuildUserPrompt(options.Category, options.Difficulty);

        const int maxValidationAttempts = 5;
        Challenge? challenge = null;

        for (int attempt = 1; attempt <= maxValidationAttempts; attempt++)
        {
            Printer.Info($"Genererar utmaning  –  Försök {attempt}/{maxValidationAttempts}");

            try
            {
                var raw = await client.GenerateAsync(prompt);
                Printer.Info("Läser JSON-svar...", indent: 1);
                var parsed = ChallengeParser.Parse(raw);

                Printer.Info("Validerar JSON-svar...", indent: 1);
                var (isValid, reason) = ChallengeValidator.Validate(parsed);

                if (isValid)
                {
                    challenge = parsed;
                    break;
                }

                Printer.Warn($"Ogiltigt svar: {reason}", indent: 1);
                if (attempt < maxValidationAttempts)
                    Printer.Dim("Begär nytt svar...", indent: 1);
            }
            catch (Exception ex)
            {
                Printer.Error(ex.Message, indent: 1);

                // Avbryt omedelbart vid terminala fel (ex. 401 Unauthorized)
                if (ex.Message.Contains("401") || ex.Message.Contains("400"))
                    return;

                if (attempt < maxValidationAttempts)
                    Printer.Dim("Försöker igen...", indent: 1);
            }
        }

        if (challenge is null)
        {
            Printer.Blank();
            Printer.Error($"Kunde inte generera en giltig övning efter {maxValidationAttempts} försök.", indent: 1);
            return;
        }

        Printer.Info("Genererar filer...", indent: 1);
        var scaffolder = new ExerciseScaffolder();
        scaffolder.Scaffold(challenge);
        
    }
}