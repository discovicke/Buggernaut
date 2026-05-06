namespace Buggernaut.Generator;

public class ExerciseScaffolder
{
    private readonly string _solutionRoot = FindSolutionRoot();

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

        File.WriteAllText(exercisePath, challenge.BuggyCode);
        File.WriteAllText(testPath, challenge.TestCode);
        File.WriteAllText(solutionPath, challenge.SolutionCode);

        Console.WriteLine($"\n Övning skapad: {className}");
        Console.WriteLine($"   Öppna:     src/Buggernaut.Exercises/{className}.cs");
        Console.WriteLine($"   Testa med: dotnet test");
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