using Buggernaut.Generator;

namespace Buggernaut.Generator.Tests;

public class PromptBuilderTests
{
    [Fact]
    public void SystemPrompt_IsNotEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(PromptBuilder.SystemPrompt));
    }

    [Fact]
    public void SystemPrompt_ContainsRequiredJsonFields()
    {
        var prompt = PromptBuilder.SystemPrompt;

        Assert.Contains("title",       prompt);
        Assert.Contains("buggyCode",   prompt);
        Assert.Contains("solutionCode",prompt);
        Assert.Contains("testCode",    prompt);
        Assert.Contains("explanation", prompt);
    }

    [Fact]
    public void SystemPrompt_ContainsNamespaceRules()
    {
        Assert.Contains("Buggernaut.Exercises", PromptBuilder.SystemPrompt);
        Assert.Contains("Buggernaut.Tests",     PromptBuilder.SystemPrompt);
    }

    [Theory]
    [InlineData(ChallengeCategories.Bug)]
    [InlineData(ChallengeCategories.GuessOutput)]
    [InlineData(ChallengeCategories.FillInTheGap)]
    [InlineData(ChallengeCategories.AlgorithmRiddle)]
    [InlineData(ChallengeCategories.LINQ)]
    [InlineData(ChallengeCategories.BlackBox)]
    [InlineData(ChallengeCategories.LogTime)]
    [InlineData(ChallengeCategories.General)]
    public void BuildUserPrompt_AllCategories_ReturnsNonEmptyString(ChallengeCategories category)
    {
        var prompt = PromptBuilder.BuildUserPrompt(category, Difficulties.Medium);

        Assert.False(string.IsNullOrWhiteSpace(prompt));
    }

    [Theory]
    [InlineData(Difficulties.Easy)]
    [InlineData(Difficulties.Medium)]
    [InlineData(Difficulties.Hard)]
    public void BuildUserPrompt_AllDifficulties_IncludesDifficultyInPrompt(Difficulties difficulty)
    {
        var prompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.Bug, difficulty);

        Assert.Contains(difficulty.ToString(), prompt);
    }

    [Fact]
    public void BuildUserPrompt_DifferentCategories_ProduceDifferentPrompts()
    {
        var bugPrompt  = PromptBuilder.BuildUserPrompt(ChallengeCategories.Bug,  Difficulties.Medium);
        var linqPrompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.LINQ, Difficulties.Medium);

        Assert.NotEqual(bugPrompt, linqPrompt);
    }

    [Fact]
    public void BuildUserPrompt_WithTopic_IncludesTopicInPrompt()
    {
        var prompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.General, Difficulties.Medium, "Fluent interface");

        Assert.Contains("Fluent interface", prompt);
    }

    [Fact]
    public void BuildUserPrompt_WithoutTopic_OmitsFocusSentence()
    {
        var prompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.General, Difficulties.Medium);

        Assert.DoesNotContain("Focus the challenge", prompt);
    }

    [Fact]
    public void BuildUserPrompt_EmptyTopic_OmitsFocusSentence()
    {
        var prompt = PromptBuilder.BuildUserPrompt(ChallengeCategories.General, Difficulties.Medium, "   ");

        Assert.DoesNotContain("Focus the challenge", prompt);
    }
}

