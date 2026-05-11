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
         - buggyCode: a complete, compilable C# class with exactly ONE intentional bug.
           - Add exactly ONE comment: // TODO: There is a bug somewhere in this class. Place it at the very TOP of the class body, NOT near the buggy line.
           - The buggy line MUST look like intentional, legitimate production code. No suspicious formatting, no unusual variable names, and absolutely NO comments near or on the buggy line.
           - Do NOT mention, describe, hint at, or reference the bug anywhere in the code — not in XML doc comments, inline comments, variable names, or method names.
           - All XML doc comments must describe intended behaviour as if the implementation is correct.
         - solutionCode: the corrected version of buggyCode. Remove the // TODO comment. All other code must be identical to buggyCode except the single fix.
         - hint: ONE sentence. Point toward the general area of the code (e.g. a method name) without naming the bug, the correct value, or the fix. Must not be solvable from the hint alone.
         - description: 3–4 sentences. Describe the real-world context and what the class is responsible for. Do NOT describe what is wrong, what to look for, or what needs fixing — the student discovers that through the tests.
         - explanation: 3–5 sentences. Explain precisely what the bug was, why it caused wrong behaviour, and what the correct fix is and why it works.
         - testCode: a valid xUnit test class. Class name must be [ClassName]Tests. At least 3 [Fact] tests.
            - Tests MUST fail on buggyCode and pass on solutionCode.
            - Tests must be internally consistent. All [Fact] tests must agree on the same behavioral contract.
            - testCode MUST use namespace Buggernaut.Tests (NOT Buggernaut.Exercises or Buggernaut.Exercises.Tests).
            - testCode must use: using Xunit; using Buggernaut.Exercises;
         - No NuGet packages, only System namespace (except Xunit in testCode).
         - Every class in buggyCode and solutionCode MUST have an XML /// <summary> that describes what the class does in a professional application (e.g. "Used by a billing service to ...").
         - Every public method in buggyCode and solutionCode MUST have an XML /// <summary> with /// <param> and /// <returns> tags where applicable.
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