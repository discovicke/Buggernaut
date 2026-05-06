namespace Buggernaut.Generator;

public class PromptBuilder
{
    public static string SystemPrompt => """
         You are a C# coding challenge generator for junior developers.
         Respond ONLY with valid JSON. No markdown, no code fences, no explanation outside the JSON.

         Always use this exact structure:
         {
             "title": "string",
             "description": "string",
             "buggyCode": "string",
             "hint": "string",
             "solutionCode": "string",
             "explanation": "string",
             "testCode": "string"
         }

         Rules:
         - buggyCode: a complete, compilable C# class with exactly ONE intentional bug. Add // TODO: Something is wrong here near the bug.
         - solutionCode: the corrected version of buggyCode.
         - testCode: a valid xUnit test class. Class name must be [ClassName]Tests. At least 3 [Fact] tests. 
         - Tests MUST fail on buggyCode and pass on solutionCode.
         - Tests must be internally consistent. All [Fact] tests must agree on the same behavioral contract.
         - All classes use namespace Buggernaut.Exercises.
         - testCode must use: using Xunit; using Buggernaut.Exercises;
         - No NuGet packages, only System namespace (except Xunit in testCode).
         """;
    
    public static string BuildUserPrompt(ChallengeCategories category, Difficulties difficulty)
    {
        var categoryDescription = category switch
        {
            ChallengeCategories.Bug             => "a method with an intentional bug to fix",
            ChallengeCategories.GuessOutput     => "a snippet where the user must predict the console output",
            ChallengeCategories.FillInTheGap    => "a method with a missing implementation the user must complete",
            ChallengeCategories.AlgorithmRiddle => "a small algorithm puzzle",
            ChallengeCategories.LINQ            => "a broken or incomplete LINQ query",
            ChallengeCategories.BlackBox        => "a method the user must reverse-engineer from its tests",
            ChallengeCategories.LogTime         => "a method the user must improve time- and log time from existing algorithm",
            ChallengeCategories.General         => "a general C# challenge",
            _                                   => "a general C# challenge"
        };
        
        return $"Generate a C# challenge for {difficulty} difficulty. The challenge should be {categoryDescription}.";
    }
}