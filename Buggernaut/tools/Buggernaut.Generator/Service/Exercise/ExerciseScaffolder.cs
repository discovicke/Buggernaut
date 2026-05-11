namespace Buggernaut.Generator;

public class ExerciseScaffolder
{
    private readonly string _solutionRoot;

    public ExerciseScaffolder(string? outputRoot = null)
    {
        _solutionRoot = outputRoot ?? SolutionRootFinder.Find();
    }

    public void Scaffold(Challenge challenge)
    {
        var className = ExtractClassName(challenge.BuggyCode);

        var exercisePath = Path.Combine(_solutionRoot, "src", "Buggernaut.Exercises", $"{className}.cs");
        var testPath     = Path.Combine(_solutionRoot, "tests", "Buggernaut.Tests", $"{className}Tests.cs");
        var solutionPath = Path.Combine(_solutionRoot, "solutions", $"{className}.cs");
        var metaPath     = Path.Combine(_solutionRoot, "solutions", $"{className}.meta.json");

        Directory.CreateDirectory(Path.GetDirectoryName(exercisePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(testPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(solutionPath)!);

        var header = BuildDescriptionHeader(challenge.Title, challenge.Description, className);

        File.WriteAllText(exercisePath, header + SanitizeNamespace(challenge.BuggyCode, "Buggernaut.Exercises"));
        File.WriteAllText(testPath,     SanitizeNamespace(challenge.TestCode,   "Buggernaut.Tests"));
        File.WriteAllText(solutionPath, SanitizeNamespace(challenge.SolutionCode, "Buggernaut.Exercises"));
        File.WriteAllText(metaPath,     SerializeMeta(challenge));

        Printer.Ok($"Övning genererad: \"{challenge.Title}\"");
        Printer.H2("Övningsinformation");
        Printer.Info(className);
        Printer.KeyValue("Övning",    $"src/Buggernaut.Exercises/{className}.cs");
        Printer.KeyValue("Test",      $"tests/Buggernaut.Tests/{className}Tests.cs");
        Printer.KeyValue("Lösning",   $"solutions/{className}.cs");
        Printer.Blank();
        Printer.Dim($"Kör testerna:  dotnet test exercises.slnf  (från Buggernaut/)", indent: 1);
        Printer.Dim($"Ledtråd:       dotnet buggernaut hint {className}", indent: 1);
        Printer.Dim($"Förklaring:    dotnet buggernaut explain {className}", indent: 1);
    }

    private static string BuildDescriptionHeader(string title, string description, string className)
    {
        const int width = 72;
        var border = "//" + new string('-', width);

        var lines = new List<string>
        {
            border,
            $"//  {title}",
            "//",
        };

        foreach (var wrapped in WrapText(description, 68))
            lines.Add($"//  {wrapped}");

        lines.Add("//");
        lines.Add($"//  Testerna:    cd Buggernaut && dotnet test exercises.slnf");
        lines.Add($"//  Ledtråd:     dotnet buggernaut hint {className}");
        lines.Add($"//  Förklaring:  dotnet buggernaut explain {className}");
        lines.Add(border);
        lines.Add("");

        return string.Join("\n", lines) + "\n";
    }

    private static IEnumerable<string> WrapText(string text, int maxWidth)
    {
        var words = text.Split(' ');
        var line = new System.Text.StringBuilder();
        foreach (var word in words)
        {
            if (line.Length + word.Length + 1 > maxWidth && line.Length > 0)
            {
                yield return line.ToString();
                line.Clear();
            }
            if (line.Length > 0) line.Append(' ');
            line.Append(word);
        }
        if (line.Length > 0) yield return line.ToString();
    }

    private static string SerializeMeta(Challenge challenge)
    {
        var meta = new
        {
            title       = challenge.Title,
            description = challenge.Description,
            hint        = challenge.Hint,
            explanation = challenge.Explanation
        };
        return System.Text.Json.JsonSerializer.Serialize(meta,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Ersätter eventuellt felaktigt namespace med det förväntade.
    /// Hanterar både block-syntax (namespace Foo {}) och fil-scoped (namespace Foo;).
    /// </summary>
    private static string SanitizeNamespace(string code, string expectedNamespace)
    {
        // Matchar: namespace Whatever.Goes.Here; eller namespace Whatever.Goes.Here {
        return System.Text.RegularExpressions.Regex.Replace(
            code,
            @"namespace\s+[\w.]+(\s*[;{])",
            $"namespace {expectedNamespace}$1"
        );
    }

    /// <summary>
    /// Regex-metod som letar efter klassnamnet i buggyCode.
    /// T.ex. "public class Foo" eller "public static class Bar"
    /// </summary>
    private string ExtractClassName(string code)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            code, @"public\s+(static\s+)?class\s+(\w+)"
        );
        return match.Success
            ? match.Groups[2].Value
            : "UnknownExercise";
    }
}