namespace Buggernaut.Generator;

/// <summary>
/// Generates a local mock Challenge for --dry-run mode.
/// Each category has its own flavour so the scaffolded files look realistic.
/// No API call is made.
/// </summary>
public static class MockChallengeFactory
{
    private record Template(
        string ClassName,
        string Description,
        string BuggyCode,
        string Hint,
        string SolutionCode,
        string Explanation,
        string TestCode);

    public static Challenge Create(ChallengeCategories category, Difficulties difficulty)
    {
        var t = category switch
        {
            ChallengeCategories.Bug             => Bug(difficulty),
            ChallengeCategories.GuessOutput     => GuessOutput(difficulty),
            ChallengeCategories.FillInTheGap    => FillInTheGap(difficulty),
            ChallengeCategories.AlgorithmRiddle => AlgorithmRiddle(difficulty),
            ChallengeCategories.LINQ            => Linq(difficulty),
            ChallengeCategories.BlackBox        => BlackBox(difficulty),
            ChallengeCategories.LogTime         => LogTime(difficulty),
            ChallengeCategories.General         => General(difficulty),
            _                                   => General(difficulty)
        };

        return new Challenge
        {
            Title        = $"[DRY-RUN] {category} – {difficulty} – {t.ClassName}",
            Description  = t.Description,
            BuggyCode    = t.BuggyCode,
            Hint         = t.Hint,
            SolutionCode = t.SolutionCode,
            Explanation  = t.Explanation,
            TestCode     = t.TestCode
        };
    }
    
    private static Template Bug(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

            """,
        Hint: " ",
        SolutionCode: """

            """,
        Explanation: " ",
        TestCode: """

            """
    );
    
    private static Template GuessOutput(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

                   """,
        Hint: " ",
        SolutionCode: """

                      """,
        Explanation: " ",
        TestCode: """

                  """
    );
    
    private static Template FillInTheGap(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

                   """,
        Hint: " ",
        SolutionCode: """

                      """,
        Explanation: " ",
        TestCode: """

                  """
    );
    
    private static Template AlgorithmRiddle(Difficulties d) => new(
        ClassName: "MockAlgorithmRiddleExercise",
        Description: $"[{d}] Implementera en metod som avgör om ett tal är ett primtal.",
        BuggyCode: """
            namespace Buggernaut.Exercises;

            public class MockAlgorithmRiddleExercise
            {
                // TODO: Implementera IsPrime – returnera true om n är ett primtal, annars false.
                public bool IsPrime(int n)
                {
                    throw new NotImplementedException();
                }
            }
            """,
        Hint: "Ett primtal är bara delbart med 1 och sig själv. Kontrollera divisorer upp till √n.",
        SolutionCode: """
            namespace Buggernaut.Exercises;

            public class MockAlgorithmRiddleExercise
            {
                public bool IsPrime(int n)
                {
                    if (n < 2) return false;
                    for (int i = 2; i * i <= n; i++)
                        if (n % i == 0) return false;
                    return true;
                }
            }
            """,
        Explanation: "Loopa divisorer från 2 upp till √n. Om n är delbart med något tal är det inte ett primtal.",
        TestCode: """
            namespace Buggernaut.Tests;
            using Buggernaut.Exercises;

            public class MockAlgorithmRiddleExerciseTests
            {
                [Fact]
                public void IsPrime_PrimeNumber_ReturnsTrue()
                {
                    var sut = new MockAlgorithmRiddleExercise();
                    Assert.True(sut.IsPrime(7));
                }

                [Fact]
                public void IsPrime_CompositeNumber_ReturnsFalse()
                {
                    var sut = new MockAlgorithmRiddleExercise();
                    Assert.False(sut.IsPrime(9));
                }

                [Fact]
                public void IsPrime_One_ReturnsFalse()
                {
                    var sut = new MockAlgorithmRiddleExercise();
                    Assert.False(sut.IsPrime(1));
                }
            }
            """
    );
    
    private static Template Linq(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

                   """,
        Hint: " ",
        SolutionCode: """

                      """,
        Explanation: " ",
        TestCode: """

                  """
    );
    
    private static Template BlackBox(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

                   """,
        Hint: " ",
        SolutionCode: """

                      """,
        Explanation: " ",
        TestCode: """

                  """
    );
    
    private static Template LogTime(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

                   """,
        Hint: " ",
        SolutionCode: """

                      """,
        Explanation: " ",
        TestCode: """

                  """
    );
    
    private static Template General(Difficulties d) => new(
        ClassName: " ",
        Description: $" ",
        BuggyCode: """

                   """,
        Hint: " ",
        SolutionCode: """

                      """,
        Explanation: " ",
        TestCode: """

                  """
    );
}
