namespace Buggernaut.Generator;

public class ExerciseScaffolder
{
    private readonly string _solutionRoot;

    public ExerciseScaffolder(string? outputRoot = null)
    {
        _solutionRoot = outputRoot ?? FindSolutionRoot();
    }

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir != null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return dir.FullName;

            dir = dir.Parent;
        }

        throw new Exception("Kunde inte hitta solution-rooten. Finns det en .sln-fil i Buggernaut/?");
    }

    public void Scaffold(Challenge challenge)
    {
        var className = ExtractClassName(challenge.BuggyCode);

        var exercisePath = Path.Combine(_solutionRoot, "src", "Buggernaut.Exercises", $"{className}.cs");
        var testPath = Path.Combine(_solutionRoot, "tests", "Buggernaut.Tests", $"{className}Tests.cs");
        var solutionPath = Path.Combine(_solutionRoot, "solutions", $"{className}.cs");

        Directory.CreateDirectory(Path.GetDirectoryName(exercisePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(testPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(solutionPath)!);

        File.WriteAllText(exercisePath, SanitizeNamespace(challenge.BuggyCode, "Buggernaut.Exercises"));
        File.WriteAllText(testPath,     SanitizeNamespace(challenge.TestCode,   "Buggernaut.Tests"));
        File.WriteAllText(solutionPath, SanitizeNamespace(challenge.SolutionCode, "Buggernaut.Exercises"));

        Printer.Ok($"Övning genererad: \"{challenge.Title}\"");
        Printer.H2("Övningsinformation");
        Printer.Info(className);
        Printer.KeyValue("Övning",   $"src/Buggernaut.Exercises/{className}.cs");
        Printer.KeyValue("Test",     $"tests/Buggernaut.Tests/{className}Tests.cs");
        Printer.KeyValue("Lösning",  $"solutions/{className}.cs");
        Printer.Blank();
        Printer.Dim("Kör testerna:  dotnet test", indent: 1);
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