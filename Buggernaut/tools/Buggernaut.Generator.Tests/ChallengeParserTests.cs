using Buggernaut.Generator;
using Xunit;

namespace Buggernaut.Generator.Tests;

public class ChallengeParserTests
{
    private const string ValidJson = """
        {
            "title": "Fix the loop",
            "description": "A simple loop bug.",
            "buggyCode": "namespace Buggernaut.Exercises; // TODO: fix me\npublic class Foo {}",
            "hint": "Check the condition.",
            "solutionCode": "namespace Buggernaut.Exercises;\npublic class Foo {}",
            "explanation": "The loop was off by one.",
            "testCode": "namespace Buggernaut.Tests;\npublic class FooTests { [Fact] public void Test() {} }"
        }
        """;

    [Fact]
    public void Parse_ValidJson_MapsAllFields()
    {
        var challenge = ChallengeParser.Parse(ValidJson);

        Assert.Equal("Fix the loop", challenge.Title);
        Assert.Equal("A simple loop bug.", challenge.Description);
        Assert.Contains("TODO", challenge.BuggyCode);
        Assert.Equal("Check the condition.", challenge.Hint);
        Assert.Contains("Foo", challenge.SolutionCode);
        Assert.Equal("The loop was off by one.", challenge.Explanation);
        Assert.Contains("[Fact]", challenge.TestCode);
    }

    [Fact]
    public void Parse_JsonWithMarkdownFences_StripsFencesAndParses()
    {
        var wrapped = $"```json\n{ValidJson}\n```";

        var challenge = ChallengeParser.Parse(wrapped);

        Assert.Equal("Fix the loop", challenge.Title);
    }

    [Fact]
    public void Parse_JsonWithPlainFences_StripsFencesAndParses()
    {
        var wrapped = $"```\n{ValidJson}\n```";

        var challenge = ChallengeParser.Parse(wrapped);

        Assert.Equal("Fix the loop", challenge.Title);
    }

    [Fact]
    public void Parse_JsonWithLeadingAndTrailingWhitespace_Parses()
    {
        var padded = $"\n\n  {ValidJson}  \n\n";

        var challenge = ChallengeParser.Parse(padded);

        Assert.Equal("Fix the loop", challenge.Title);
    }

    [Fact]
    public void Parse_PropertiesAreCaseInsensitive()
    {
        var upperCaseJson = """
            {
                "TITLE": "Upper case",
                "DESCRIPTION": "desc",
                "BUGGYCODE": "code",
                "HINT": "hint",
                "SOLUTIONCODE": "sol",
                "EXPLANATION": "exp",
                "TESTCODE": "test"
            }
            """;

        var challenge = ChallengeParser.Parse(upperCaseJson);

        Assert.Equal("Upper case", challenge.Title);
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => ChallengeParser.Parse("not json at all"));
    }

    [Fact]
    public void Parse_EmptyObject_ReturnsChallengeWithEmptyFields()
    {
        var challenge = ChallengeParser.Parse("{}");

        Assert.Equal("", challenge.Title);
        Assert.Equal("", challenge.BuggyCode);
    }
}

