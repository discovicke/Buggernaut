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
        "explanation": "string"
    }
        
    Rules:
    - buggyCode must be a complete, compilable C# class with exactly ONE bug.
    - Add a comment saying // TODO: Something is wrong here near the bug.
    - solutionCode is the corrected version of buggyCode.
    - Use namespace Buggernaut.Exercises for all generated classes.
    - Do not use any NuGet packages, only System namespace.
    """;
    
    public static string BuildUserPrompt(ChallengeCategories category, Difficulties difficulty)
    {
        var categoryDescription = category switch
        {
            ChallengeCategories.Bug           => "a method with an intentional bug to fix",
            ChallengeCategories.GuessOutput   => "a snippet where the user must predict the console output",
            ChallengeCategories.FillInTheGap  => "a method with a missing implementation the user must complete",
            ChallengeCategories.AlgorithmRiddle => "a small algorithm puzzle",
            ChallengeCategories.LINQ          => "a broken or incomplete LINQ query",
            ChallengeCategories.BlackBox      => "a method the user must reverse-engineer from its tests",
            _ => "a general C# challenge"
        };
        
        return $"Generate a C# challenge for {difficulty} difficulty. The challenge should be {categoryDescription}.";
    }
}