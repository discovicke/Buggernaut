using System.Text;

namespace Buggernaut.Generator;

/// <summary>
/// Parses and validates CLI arguments for the generate command.
/// Usage: dotnet run -- generate --category --difficulty
/// </summary>
public static class CliArgumentParser
{
    public class GenerateOptions
    {
        public ChallengeCategories Category { get; set; } = ChallengeCategories.Bug;
        public Difficulties Difficulty { get; set; } = Difficulties.Medium;
    }

    public static GenerateOptions Parse(string[] args)
    {
        if (args.Length == 0)
            return GetDefaults();

        if (args[0] != "generate")
            throw new CliArgumentException($"Okänt kommando: '{args[0]}'. Använd: dotnet run -- generate [flaggor]");

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
        var help = new StringBuilder();
        help.AppendLine("\n Buggernaut Generator – Hjälp\n");
        help.AppendLine("Användning:");
        help.AppendLine("  dotnet run -- generate [flaggor]\n");
        help.AppendLine("Flaggor:");
        help.AppendLine("  --category,\t-c <category>\tTyp av övning");
        help.AppendLine("  --difficulty,\t-d <difficulty>\tSvårighetsgrad (standard: Medium)");
        help.AppendLine("  --list,\t-l\t\tVisa tillgängliga kategorier och svårighetsgrader");
        help.AppendLine("  --help,\t-h\t\tVisa denna hjälptexten\n");
        help.AppendLine("Tillgängliga kategorier:");
        help.Append("  ");
        help.AppendLine(string.Join(", ", Enum.GetNames(typeof(ChallengeCategories))));
        help.AppendLine("\nTillgängliga svårighetsgrader:");
        help.Append("  ");
        help.AppendLine(string.Join(", ", Enum.GetNames(typeof(Difficulties))));
        help.AppendLine("\nExempel:");
        help.AppendLine("  dotnet run -- generate");
        help.AppendLine("  dotnet run -- generate --category Bug --difficulty Hard");
        help.AppendLine("  dotnet run -- generate -c LINQ -d Easy");
        help.AppendLine("  dotnet run -- generate --list");

        Console.WriteLine(help);
    }

    private static void PrintList()
    {
        var list = new StringBuilder();
        list.AppendLine("\nBuggernaut – Tillgängliga utmaningar\n");

        list.AppendLine("Kategorier:");
        list.Append("  ");
        list.AppendLine(string.Join(", ", Enum.GetNames(typeof(ChallengeCategories))));

        list.AppendLine("\nSvårighetsgrader:");
        list.Append("  ");
        list.AppendLine(string.Join(", ", Enum.GetNames(typeof(Difficulties))));

        list.AppendLine("\nAnvändning:");
        list.AppendLine("  dotnet run -- generate --category <category> --difficulty <difficulty>");
        list.AppendLine("  dotnet run -- generate -c Bug -d Hard");

        Console.WriteLine(list);
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