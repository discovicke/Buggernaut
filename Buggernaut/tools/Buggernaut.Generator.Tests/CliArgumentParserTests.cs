using Buggernaut.Generator;
using Xunit;

namespace Buggernaut.Generator.Tests;

public class CliArgumentParserTests : IDisposable
{
    private readonly TextWriter _originalOut;

    public CliArgumentParserTests()
    {
        _originalOut = Console.Out;
        Console.SetOut(TextWriter.Null);
    }

    public void Dispose() => Console.SetOut(_originalOut);

    [Fact]
    public void Parse_NoArgs_ReturnsDefaultOptions()
    {
        var options = CliArgumentParser.Parse([]);

        Assert.False(options.DryRun);
    }


    [Fact]
    public void Parse_LongFlags_ParsesCategoryAndDifficulty()
    {
        var options = CliArgumentParser.Parse(["generate", "--category", "Bug", "--difficulty", "Hard"]);

        Assert.Equal(ChallengeCategories.Bug, options.Category);
        Assert.Equal(Difficulties.Hard, options.Difficulty);
    }

    [Fact]
    public void Parse_ShortFlags_ParsesCategoryAndDifficulty()
    {
        var options = CliArgumentParser.Parse(["generate", "-c", "LINQ", "-d", "Easy"]);

        Assert.Equal(ChallengeCategories.LINQ, options.Category);
        Assert.Equal(Difficulties.Easy, options.Difficulty);
    }

    [Fact]
    public void Parse_CategoryIsCaseInsensitive()
    {
        var options = CliArgumentParser.Parse(["generate", "--category", "algorithmriddle"]);

        Assert.Equal(ChallengeCategories.AlgorithmRiddle, options.Category);
    }

    [Fact]
    public void Parse_DryRunFlag_SetsDryRunTrue()
    {
        var options = CliArgumentParser.Parse(["generate", "--dry-run"]);

        Assert.True(options.DryRun);
    }

    [Fact]
    public void Parse_ShortDryRunFlag_SetsDryRunTrue()
    {
        var options = CliArgumentParser.Parse(["generate", "-dr"]);

        Assert.True(options.DryRun);
    }


    [Fact]
    public void Parse_HelpFlag_ThrowsNonErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--help"]));

        Assert.False(ex.IsError);
    }

    [Fact]
    public void Parse_ShortHelpFlag_ThrowsNonErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "-h"]));

        Assert.False(ex.IsError);
    }

    [Fact]
    public void Parse_ListFlag_ThrowsNonErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--list"]));

        Assert.False(ex.IsError);
    }


    [Fact]
    public void Parse_UnknownCommand_ThrowsErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["unknown"]));

        Assert.True(ex.IsError);
    }

    [Fact]
    public void Parse_UnknownFlag_ThrowsErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--unknown"]));

        Assert.True(ex.IsError);
    }

    [Fact]
    public void Parse_CategoryFlagWithoutValue_ThrowsErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--category"]));

        Assert.True(ex.IsError);
    }

    [Fact]
    public void Parse_DifficultyFlagWithoutValue_ThrowsErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--difficulty"]));

        Assert.True(ex.IsError);
    }

    [Fact]
    public void Parse_InvalidCategoryValue_ThrowsErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--category", "NotACategory"]));

        Assert.True(ex.IsError);
        Assert.Contains("NotACategory", ex.Message);
    }

    [Fact]
    public void Parse_InvalidDifficultyValue_ThrowsErrorException()
    {
        var ex = Assert.Throws<CliArgumentException>(
            () => CliArgumentParser.Parse(["generate", "--difficulty", "Extreme"]));

        Assert.True(ex.IsError);
        Assert.Contains("Extreme", ex.Message);
    }
}

