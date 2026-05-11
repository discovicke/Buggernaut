namespace Buggernaut.Generator;

/// <summary>
/// Parses and validates CLI arguments for the generate command.
/// Usage: dotnet run -- generate --category --difficulty
/// </summary>
public static class CliArgumentParser
{
    public class GenerateOptions
    {
        public Command Command { get; set; } = Command.Generate;
        public string TargetClassName { get; set; } = "";
        public ChallengeCategories Category { get; set; } = ChallengeCategories.Bug;
        public Difficulties Difficulty { get; set; } = Difficulties.Medium;
        public bool DryRun { get; set; } = false;
    }

    public static GenerateOptions Parse(string[] args)
    {
        if (args.Length == 0)
            return GetDefaults();

        if (args[0] == "hint" || args[0] == "explain")
        {
            if (args.Length < 2)
                throw new CliArgumentException(
                    $"{args[0]} kräver ett klassnamn. Exempel: dotnet run -- {args[0]} OrderValidator");

            return new GenerateOptions
            {
                Command = args[0] == "hint" 
                    ? Command.Hint 
                    : Command.Explain,
                TargetClassName = args[1]
            };
        }

        if (args[0] != "generate")
            throw new CliArgumentException(
                $"Okänt kommando: '{args[0]}'. Använd: dotnet run -- generate [flaggor]");

        var options = GetDefaults();

        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == "--category" || args[i] == "-c")
            {
                if (i + 1 >= args.Length)
                    throw new CliArgumentException("--category kräver ett värde. Exempel: --category Bug");

                var categoryValue = args[++i];
                options.Category = ParseCategory(categoryValue);
            }
            else if (args[i] == "--difficulty" || args[i] == "-d")
            {
                if (i + 1 >= args.Length)
                    throw new CliArgumentException("--difficulty kräver ett värde. Exempel: --difficulty Hard");

                var difficultyValue = args[++i];
                options.Difficulty = ParseDifficulty(difficultyValue);
            }
            else if (args[i] == "--dry-run" || args[i] == "-dr")
            {
                options.DryRun = true;
            }
            else if (args[i] == "--list" || args[i] == "-l")
            {
                PrintList();
                throw new CliArgumentException("Visade lista.", isError: false);
            }
            else if (args[i] == "--help" || args[i] == "-h")
            {
                PrintHelp();
                throw new CliArgumentException("Visat hjälp.", isError: false);
            }
            else
            {
                throw new CliArgumentException(
                    $"Okänd flagga: '{args[i]}'. Använd --help för att se tillgängliga flaggor.");
            }
        }

        return options;
    }

    private static GenerateOptions GetDefaults()
    {
        return new GenerateOptions
        {
            Category = ChallengeCategories.BlackBox,
            Difficulty = Difficulties.Medium
        };
    }

    private static ChallengeCategories ParseCategory(string value)
    {
        if (Enum.TryParse<ChallengeCategories>(value, ignoreCase: true, out var category))
            return category;

        var available = string.Join(", ", Enum.GetNames(typeof(ChallengeCategories)));
        throw new CliArgumentException(
            $"Ogiltigt värde för --category: '{value}'.\nTillgängliga kategorier: {available}");
    }

    private static Difficulties ParseDifficulty(string value)
    {
        if (Enum.TryParse<Difficulties>(value, ignoreCase: true, out var difficulty))
            return difficulty;

        var available = string.Join(", ", Enum.GetNames(typeof(Difficulties)));
        throw new CliArgumentException(
            $"Ogiltigt värde för --difficulty: '{value}'.\nTillgängliga svårighetsgrader: {available}");
    }

    private static void PrintHelp()
    {
        Printer.H2("Användning");
        Printer.Line("buggernaut generate [flaggor]", indent: 1);

        Printer.H2("Flaggor");
        Printer.Flag("--category, -c <category>",    "Typ av övning");
        Printer.Flag("--difficulty, -d <difficulty>", "Svårighetsgrad  (standard: Medium)");
        Printer.Flag("--dry-run, -dr",               "Lokal mock-övning, ingen API-nyckel krävs");
        Printer.Flag("--list, -l",                   "Visa tillgängliga kategorier och svårighetsgrader");
        Printer.Flag("--help, -h",                   "Visa denna hjälptext");

        Printer.H2("Kategorier");
        Printer.Line(string.Join("  |  ", Enum.GetNames(typeof(ChallengeCategories))), indent: 1);

        Printer.H2("Svårighetsgrader");
        Printer.Line(string.Join("  |  ", Enum.GetNames(typeof(Difficulties))), indent: 1);

        Printer.H2("Exempel");
        Printer.Dim("buggernaut generate", indent: 1);
        Printer.Dim("buggernaut generate --category Bug --difficulty Hard", indent: 1);
        Printer.Dim("buggernaut generate -c LINQ -d Easy", indent: 1);
        Printer.Dim("buggernaut generate --dry-run", indent: 1);
        Printer.Dim("buggernaut generate --list", indent: 1);

        Printer.H2("Kör dina övningar");
        Printer.Dim("dotnet test exercises.slnf", indent: 1);
        Printer.Dim("buggernaut hint    <ClassName>", indent: 1);
        Printer.Dim("buggernaut explain <ClassName>", indent: 1);
        Printer.Blank();
    }

    private static void PrintList()
    {

        Printer.H2("Kategorier");
        foreach (var name in Enum.GetNames(typeof(ChallengeCategories)))
            Printer.Line($"  {name}");

        Printer.H2("Svårighetsgrader");
        foreach (var name in Enum.GetNames(typeof(Difficulties)))
            Printer.Line($"  {name}");

        Printer.H2("Exempel");
        Printer.Dim("buggernaut generate --category Bug --difficulty Hard", indent: 1);
        Printer.Dim("buggernaut generate -c LINQ -d Easy", indent: 1);
        Printer.Blank();
    }
}

/// <summary>
/// Exception thrown when CLI argument parsing fails.
/// </summary>
public class CliArgumentException : Exception
{
    public bool IsError { get; }

    public CliArgumentException(string message, bool isError = true) : base(message)
    {
        IsError = isError;
    }
}