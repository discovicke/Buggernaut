using Buggernaut.Generator;
using Xunit;

namespace Buggernaut.Generator.Tests;

public class ChallengeValidatorTests
{
    private static Challenge ValidChallenge() => new()
    {
        Title       = "Fix the loop",
        Description = "A loop has an off-by-one error.",
        BuggyCode   = "namespace Buggernaut.Exercises;\npublic class Foo { // TODO: something is wrong here\n}",
        Hint        = "Check the loop condition.",
        SolutionCode = "namespace Buggernaut.Exercises;\npublic class Foo {}",
        Explanation = "The loop was off by one.",
        TestCode    = "namespace Buggernaut.Tests;\npublic class FooTests { [Fact] public void Test() {} }"
    };

    [Fact]
    public void Validate_CompleteChallenge_ReturnsValid()
    {
        var (isValid, reason) = ChallengeValidator.Validate(ValidChallenge());

        Assert.True(isValid);
        Assert.Equal("OK", reason);
    }

    [Theory]
    [InlineData(nameof(Challenge.Title),       "Titel saknas")]
    [InlineData(nameof(Challenge.Description), "Beskrivning saknas")]
    [InlineData(nameof(Challenge.BuggyCode),   "Buggy code saknas")]
    [InlineData(nameof(Challenge.Hint),        "Hint saknas")]
    [InlineData(nameof(Challenge.SolutionCode),"Solution code saknas")]
    [InlineData(nameof(Challenge.Explanation), "Explanation saknas")]
    [InlineData(nameof(Challenge.TestCode),    "Test code saknas")]
    public void Validate_MissingField_ReturnsInvalid(string fieldName, string expectedReason)
    {
        var challenge = ValidChallenge();

        // Clear the given field via reflection
        typeof(Challenge).GetProperty(fieldName)!.SetValue(challenge, "");

        var (isValid, reason) = ChallengeValidator.Validate(challenge);

        Assert.False(isValid);
        Assert.Equal(expectedReason, reason);
    }

    [Fact]
    public void Validate_TestCodeWithoutFactAttribute_ReturnsInvalid()
    {
        var challenge = ValidChallenge();
        challenge.TestCode = "namespace Buggernaut.Tests;\npublic class FooTests { public void Test() {} }";

        var (isValid, reason) = ChallengeValidator.Validate(challenge);

        Assert.False(isValid);
        Assert.Equal("TestCode innehåller inga [Fact]-tester", reason);
    }

    [Fact]
    public void Validate_BuggyCodeWithWrongNamespace_ReturnsInvalid()
    {
        var challenge = ValidChallenge();
        challenge.BuggyCode = "namespace MyApp; // TODO: fix\npublic class Foo {}";

        var (isValid, reason) = ChallengeValidator.Validate(challenge);

        Assert.False(isValid);
        Assert.Equal("BuggyCode använder fel namespace", reason);
    }

    [Fact]
    public void Validate_BuggyCodeWithoutTodoComment_ReturnsInvalid()
    {
        var challenge = ValidChallenge();
        challenge.BuggyCode = "namespace Buggernaut.Exercises;\npublic class Foo {}";

        var (isValid, reason) = ChallengeValidator.Validate(challenge);

        Assert.False(isValid);
        Assert.Equal("BuggyCode saknar TODO-kommentar", reason);
    }
}

