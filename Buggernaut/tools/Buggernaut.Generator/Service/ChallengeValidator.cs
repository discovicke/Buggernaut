namespace Buggernaut.Generator;

public static class ChallengeValidator
{
    public static (bool isValid, string reason) Validate(Challenge challenge)
    {
        if (string.IsNullOrWhiteSpace(challenge.Title))
            return (false, "Titel saknas");

        if (string.IsNullOrWhiteSpace(challenge.Description))
            return (false, "Beskrivning saknas");

        if (string.IsNullOrWhiteSpace(challenge.BuggyCode))
            return (false, "Buggy code saknas");

        if (string.IsNullOrWhiteSpace(challenge.Hint))
            return (false, "Hint saknas");

        if (string.IsNullOrWhiteSpace(challenge.SolutionCode))
            return (false, "Solution code saknas");

        if (string.IsNullOrWhiteSpace(challenge.Explanation))
            return (false, "Explanation saknas");

        if (string.IsNullOrWhiteSpace(challenge.TestCode))
            return (false, "Test code saknas");
        
        if (!challenge.TestCode.Contains("[Fact]"))
            return (false, "TestCode innehåller inga [Fact]-tester");

        if (!challenge.BuggyCode.Contains("namespace Buggernaut.Exercises"))
            return (false, "BuggyCode använder fel namespace");

        if (!challenge.BuggyCode.Contains("// TODO"))
            return (false, "BuggyCode saknar TODO-kommentar");

        return (true, "OK");
    }
}