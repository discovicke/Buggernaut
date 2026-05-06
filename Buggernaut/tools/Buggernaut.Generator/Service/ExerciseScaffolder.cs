namespace Buggernaut.Generator;

public class ExerciseScaffolder
{
    private readonly string _solutionRoot;

    public ExerciseScaffolder()
    {
        _solutionRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..")
        );
    }
    
    public void Scaffold(Challenge challenge)
    {
        var exercisePath = Path.Combine(_rootPath, "src", "Exercises", challenge.Title);
        var testPath = Path.Combine(_rootPath, "tests", challenge.Title);
        var solutionPath = Path.Combine(_rootPath, "src", "Solutions", challenge.Title);
        
        Directory.CreateDirectory(exercisePath);
        Directory.CreateDirectory(testPath);
        Directory.CreateDirectory(solutionPath);
        
        File.WriteAllText(exercisePath, challenge.BuggyCode);
        File.WriteAllText(testPath,     challenge.TestCode);
        File.WriteAllText(solutionPath, challenge.SolutionCode);

        Console.WriteLine($"\n Övning skapad: {challenge.Title}");
        Console.WriteLine($"   Öppna:     src/Buggernaut.Exercises/{challenge.Title}.cs");
        Console.WriteLine($"   Testa med: dotnet test");
    }
}