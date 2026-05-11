using Buggernaut.Generator;
using Xunit;

namespace Buggernaut.Generator.Tests;

public class ExerciseScaffolderTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly ExerciseScaffolder _scaffolder;
    private readonly TextWriter _originalOut;

    public ExerciseScaffolderTests()
    {
        _originalOut = Console.Out;
        Console.SetOut(TextWriter.Null);

        _tempRoot   = Path.Combine(Path.GetTempPath(), $"BuggernautTests_{Guid.NewGuid():N}");
        _scaffolder = new ExerciseScaffolder(_tempRoot);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOut);
        if (Directory.Exists(_tempRoot))
            Directory.Delete(_tempRoot, recursive: true);
    }

    private static Challenge MakeChallenge(string className = "Calculator") => new()
    {
        Title        = "Test challenge",
        Description  = "A test.",
        BuggyCode    = $"namespace Buggernaut.Exercises;\n// TODO: fix\npublic class {className} {{}}",
        SolutionCode = $"namespace Buggernaut.Exercises;\npublic class {className} {{}}",
        TestCode     = $"namespace Buggernaut.Tests;\npublic class {className}Tests {{ [Fact] public void T() {{}} }}",
        Hint         = "hint",
        Explanation  = "explanation"
    };


    [Fact]
    public void Scaffold_CreatesExerciseFileAtCorrectPath()
    {
        _scaffolder.Scaffold(MakeChallenge());

        var expected = Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "Calculator.cs");
        Assert.True(File.Exists(expected));
    }

    [Fact]
    public void Scaffold_CreatesTestFileAtCorrectPath()
    {
        _scaffolder.Scaffold(MakeChallenge());

        var expected = Path.Combine(_tempRoot, "tests", "Buggernaut.Tests", "CalculatorTests.cs");
        Assert.True(File.Exists(expected));
    }

    [Fact]
    public void Scaffold_CreatesSolutionFileAtCorrectPath()
    {
        _scaffolder.Scaffold(MakeChallenge());

        var expected = Path.Combine(_tempRoot, "solutions", "Calculator.cs");
        Assert.True(File.Exists(expected));
    }


    [Fact]
    public void Scaffold_ExerciseFileContainsBuggyCode()
    {
        _scaffolder.Scaffold(MakeChallenge());

        var content = File.ReadAllText(
            Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "Calculator.cs"));

        Assert.Contains("TODO", content);
        Assert.Contains("Calculator", content);
    }

    [Fact]
    public void Scaffold_SolutionFileContainsSolutionCode()
    {
        _scaffolder.Scaffold(MakeChallenge());

        var content = File.ReadAllText(
            Path.Combine(_tempRoot, "solutions", "Calculator.cs"));

        Assert.DoesNotContain("TODO", content);
        Assert.Contains("Calculator", content);
    }


    [Fact]
    public void Scaffold_WrongNamespaceInBuggyCode_IsReplacedWithCorrectNamespace()
    {
        var challenge = MakeChallenge();
        challenge.BuggyCode = "namespace WrongApp; // TODO: fix\npublic class Calculator {}";

        _scaffolder.Scaffold(challenge);

        var content = File.ReadAllText(
            Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "Calculator.cs"));

        Assert.Contains("namespace Buggernaut.Exercises", content);
        Assert.DoesNotContain("namespace WrongApp", content);
    }

    [Fact]
    public void Scaffold_BlockStyleNamespace_IsAlsoSanitized()
    {
        var challenge = MakeChallenge();
        challenge.BuggyCode = "namespace WrongApp { // TODO: fix\npublic class Calculator {} }";

        _scaffolder.Scaffold(challenge);

        var content = File.ReadAllText(
            Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "Calculator.cs"));

        Assert.Contains("namespace Buggernaut.Exercises", content);
    }

    [Fact]
    public void Scaffold_WrongNamespaceInTestCode_IsReplacedWithTestsNamespace()
    {
        var challenge = MakeChallenge();
        challenge.TestCode = "namespace WrongTests;\npublic class CalculatorTests { [Fact] public void T() {} }";

        _scaffolder.Scaffold(challenge);

        var content = File.ReadAllText(
            Path.Combine(_tempRoot, "tests", "Buggernaut.Tests", "CalculatorTests.cs"));

        Assert.Contains("namespace Buggernaut.Tests", content);
        Assert.DoesNotContain("namespace WrongTests", content);
    }


    [Fact]
    public void Scaffold_StaticClass_ExtractsClassNameCorrectly()
    {
        var challenge = MakeChallenge();
        challenge.BuggyCode    = "namespace Buggernaut.Exercises;\n// TODO: fix\npublic static class MathHelper {}";
        challenge.SolutionCode = "namespace Buggernaut.Exercises;\npublic static class MathHelper {}";
        challenge.TestCode     = "namespace Buggernaut.Tests;\npublic class MathHelperTests { [Fact] public void T() {} }";

        _scaffolder.Scaffold(challenge);

        Assert.True(File.Exists(Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "MathHelper.cs")));
    }

    [Fact]
    public void Scaffold_NoClassInCode_UsesUnknownExerciseFallback()
    {
        var challenge = MakeChallenge();
        challenge.BuggyCode    = "namespace Buggernaut.Exercises;\n// TODO: no class here";
        challenge.SolutionCode = "namespace Buggernaut.Exercises;";
        challenge.TestCode     = "namespace Buggernaut.Tests;\npublic class UnknownExerciseTests { [Fact] public void T() {} }";

        _scaffolder.Scaffold(challenge);

        Assert.True(File.Exists(Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "UnknownExercise.cs")));
    }
    
    [Fact]
    public void Scaffold_CreatesMetaFileAtCorrectPath()
    {
        _scaffolder.Scaffold(MakeChallenge());
        Assert.True(File.Exists(Path.Combine(_tempRoot, "solutions", "Calculator.meta.json")));
    }

    [Fact]
    public void Scaffold_MetaFileContainsHintAndExplanation()
    {
        _scaffolder.Scaffold(MakeChallenge());
        var json = File.ReadAllText(Path.Combine(_tempRoot, "solutions", "Calculator.meta.json"));
        Assert.Contains("hint", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("explanation", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Scaffold_ExerciseFileContainsDescriptionHeader()
    {
        _scaffolder.Scaffold(MakeChallenge());
        var content = File.ReadAllText(
            Path.Combine(_tempRoot, "src", "Buggernaut.Exercises", "Calculator.cs"));
        Assert.Contains("Test challenge", content); // title från MakeChallenge()
        Assert.Contains("A test.", content);        // description från MakeChallenge()
        Assert.Contains("dotnet run -- hint Calculator", content);
    }
}

