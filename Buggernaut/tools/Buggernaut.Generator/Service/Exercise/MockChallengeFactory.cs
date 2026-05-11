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
            ChallengeCategories.Bug => Bug(difficulty),
            ChallengeCategories.GuessOutput => GuessOutput(difficulty),
            ChallengeCategories.FillInTheGap => FillInTheGap(difficulty),
            ChallengeCategories.AlgorithmRiddle => AlgorithmRiddle(difficulty),
            ChallengeCategories.LINQ => Linq(difficulty),
            ChallengeCategories.BlackBox => BlackBox(difficulty),
            ChallengeCategories.LogTime => LogTime(difficulty),
            ChallengeCategories.General => General(difficulty),
            _ => General(difficulty)
        };

        return new Challenge
        {
            Title = $"[DRY-RUN] {category} – {difficulty} – {t.ClassName}",
            Description = t.Description,
            BuggyCode = t.BuggyCode,
            Hint = t.Hint,
            SolutionCode = t.SolutionCode,
            Explanation = t.Explanation,
            TestCode = t.TestCode
        };
    }

    private static Template Bug(Difficulties d) => d switch
    {
        Difficulties.Easy => BugEasy(),
        Difficulties.Medium => BugMedium(),
        Difficulties.Hard => BugHard(),
        _ => BugEasy()
    };

    private static Template BugEasy() => new(
        ClassName: "BugEasyVowelCounter",
        Description:
        "In a text-analysis service, vowel frequency is used to score readability and detect language patterns. " +
        "This method should count all vowel characters (a, e, i, o, u) in a given string using a case-insensitive comparison. " +
        "The current implementation contains a subtle bug that causes it to return an incorrect count for many inputs. " +
        "Identify the mistake and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Provides vowel-frequency analysis for text-processing pipelines.
                   /// Used by readability-scoring services to measure linguistic properties of strings.
                   /// </summary>
                   public class BugEasyVowelCounter
                   {
                       /// <summary>
                       /// Counts the total number of vowel characters in the supplied text.
                       /// The comparison is case-insensitive.
                       /// </summary>
                       /// <param name="text">The input string to analyse.</param>
                       /// <returns>The number of vowels found in <paramref name="text"/>.</returns>
                       // TODO: Something is wrong here
                       public int CountVowels(string text)
                       {
                           int count = 0;
                           foreach (char c in text.ToLower())
                               if ("aeio".Contains(c))
                                   count++;
                           return count;
                       }
                   }
                   """,
        Hint: "Compare the vowel string with the full set of English vowels.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Provides vowel-frequency analysis for text-processing pipelines.
                      /// Used by readability-scoring services to measure linguistic properties of strings.
                      /// </summary>
                      public class BugEasyVowelCounter
                      {
                          /// <summary>
                          /// Counts the total number of vowel characters in the supplied text.
                          /// The comparison is case-insensitive.
                          /// </summary>
                          /// <param name="text">The input string to analyse.</param>
                          /// <returns>The number of vowels found in <paramref name="text"/>.</returns>
                          public int CountVowels(string text)
                          {
                              int count = 0;
                              foreach (char c in text.ToLower())
                                  if ("aeiou".Contains(c))
                                      count++;
                              return count;
                          }
                      }
                      """,
        Explanation:
        "The vowel string \"aeio\" is missing the character 'u', so any word containing 'u' (such as \"sun\" or \"umbrella\") " +
        "would have its vowels undercounted. " +
        "Because the check is performed on every character, exactly one false-negative occurs per 'u' in the input. " +
        "The fix is to change the string literal to \"aeiou\" so all five English vowels are included.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class BugEasyVowelCounterTests
                  {
                      [Fact]
                      public void CountVowels_WordWithU_CountsItCorrectly()
                      {
                          var sut = new BugEasyVowelCounter();
                          Assert.Equal(1, sut.CountVowels("sun"));
                      }

                      [Fact]
                      public void CountVowels_MultipleVowelsIncludingU_ReturnsCorrectCount()
                      {
                          var sut = new BugEasyVowelCounter();
                          Assert.Equal(3, sut.CountVowels("umbrella"));
                      }

                      [Fact]
                      public void CountVowels_NoVowels_ReturnsZero()
                      {
                          var sut = new BugEasyVowelCounter();
                          Assert.Equal(0, sut.CountVowels("gym"));
                      }

                      [Fact]
                      public void CountVowels_EmptyString_ReturnsZero()
                      {
                          var sut = new BugEasyVowelCounter();
                          Assert.Equal(0, sut.CountVowels(""));
                      }
                  }
                  """
    );

    private static Template BugMedium() => new(
        ClassName: "BugMediumWordCounter",
        Description:
        "In a search-engine indexing service, word-frequency maps are built from documents to power relevance ranking and full-text search. " +
        "This method should return a Dictionary mapping each whitespace-delimited word in a sentence to its occurrence count. " +
        "The current implementation contains a bug that causes every word's initial count to be recorded incorrectly. " +
        "Find and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Analyses word frequency in natural-language text.
                   /// Used by search-engine indexing pipelines to build term-frequency maps for document ranking.
                   /// </summary>
                   public class BugMediumWordCounter
                   {
                       /// <summary>
                       /// Counts the number of times each whitespace-delimited word appears in the sentence.
                       /// The comparison is case-sensitive.
                       /// </summary>
                       /// <param name="sentence">The input sentence to analyse.</param>
                       /// <returns>
                       /// A dictionary mapping each distinct word to its occurrence count.
                       /// </returns>
                       // TODO: Something is wrong here
                       public Dictionary<string, int> CountWords(string sentence)
                       {
                           var result = new Dictionary<string, int>();
                           foreach (var word in sentence.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
                           {
                               if (result.ContainsKey(word))
                                   result[word]++;
                               else
                                   result[word] = 0;
                           }
                           return result;
                       }
                   }
                   """,
        Hint: "What should a word that appears for the first time be initialized to?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Analyses word frequency in natural-language text.
                      /// Used by search-engine indexing pipelines to build term-frequency maps for document ranking.
                      /// </summary>
                      public class BugMediumWordCounter
                      {
                          /// <summary>
                          /// Counts the number of times each whitespace-delimited word appears in the sentence.
                          /// The comparison is case-sensitive.
                          /// </summary>
                          /// <param name="sentence">The input sentence to analyse.</param>
                          /// <returns>
                          /// A dictionary mapping each distinct word to its occurrence count.
                          /// </returns>
                          public Dictionary<string, int> CountWords(string sentence)
                          {
                              var result = new Dictionary<string, int>();
                              foreach (var word in sentence.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
                              {
                                  if (result.ContainsKey(word))
                                      result[word]++;
                                  else
                                      result[word] = 1;
                              }
                              return result;
                          }
                      }
                      """,
        Explanation:
        "When a word was encountered for the first time, it was initialised to 0 instead of 1. " +
        "This means every word whose count is read before a second occurrence will appear to have been seen zero times rather than once. " +
        "Because the increment path (result[word]++) is only reached on the second and subsequent visits, " +
        "the very first occurrence was silently lost. " +
        "The fix is to initialise new entries to 1 so the first sighting is counted correctly.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class BugMediumWordCounterTests
                  {
                      [Fact]
                      public void CountWords_SingleWord_ReturnsOne()
                      {
                          var sut = new BugMediumWordCounter();
                          var result = sut.CountWords("hello");
                          Assert.Equal(1, result["hello"]);
                      }

                      [Fact]
                      public void CountWords_RepeatedWord_ReturnsCorrectCount()
                      {
                          var sut = new BugMediumWordCounter();
                          var result = sut.CountWords("a b a");
                          Assert.Equal(2, result["a"]);
                          Assert.Equal(1, result["b"]);
                      }

                      [Fact]
                      public void CountWords_AllUniqueWords_AllReturnOne()
                      {
                          var sut = new BugMediumWordCounter();
                          var result = sut.CountWords("one two three");
                          Assert.Equal(1, result["one"]);
                          Assert.Equal(1, result["two"]);
                          Assert.Equal(1, result["three"]);
                      }
                  }
                  """
    );

    private static Template BugHard() => new(
        ClassName: "BugHardBracketValidator",
        Description:
        "Parser infrastructure in compilers, JSON deserializers, and expression evaluators all rely on bracket-balance validation to reject malformed input early. " +
        "This method should return true if every opening bracket in a string has a correctly matched closing counterpart and vice versa. " +
        "The current implementation passes for parentheses and curly braces but silently misclassifies certain square-bracket expressions. " +
        "Find the subtle character-matching bug and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Validates that bracket characters in a string are correctly nested and matched.
                   /// Used by parser infrastructure in compilers and expression evaluators to reject malformed input.
                   /// </summary>
                   public class BugHardBracketValidator
                   {
                       /// <summary>
                       /// Determines whether every opening bracket in the input has a matching closing bracket
                       /// in the correct order, supporting '()', '[]', and '{}'.
                       /// </summary>
                       /// <param name="input">The string to validate.</param>
                       /// <returns><c>true</c> if all brackets are correctly balanced; otherwise <c>false</c>.</returns>
                       // TODO: Something is wrong here
                       public bool IsBalanced(string input)
                       {
                           var stack = new Stack<char>();
                           foreach (char c in input)
                           {
                               if (c is '(' or '[' or '{')
                                   stack.Push(c);
                               else if (c is ')' or ']' or '}')
                               {
                                   if (stack.Count == 0) return false;
                                   var top = stack.Pop();
                                   if ((c == ')' && top != '(') ||
                                       (c == ']' && top != '{') ||
                                       (c == '}' && top != '{'))
                                       return false;
                               }
                           }
                           return stack.Count == 0;
                       }
                   }
                   """,
        Hint: "Check which character should match ']'. Is the condition correct?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Validates that bracket characters in a string are correctly nested and matched.
                      /// Used by parser infrastructure in compilers and expression evaluators to reject malformed input.
                      /// </summary>
                      public class BugHardBracketValidator
                      {
                          /// <summary>
                          /// Determines whether every opening bracket in the input has a matching closing bracket
                          /// in the correct order, supporting '()', '[]', and '{}'.
                          /// </summary>
                          /// <param name="input">The string to validate.</param>
                          /// <returns><c>true</c> if all brackets are correctly balanced; otherwise <c>false</c>.</returns>
                          public bool IsBalanced(string input)
                          {
                              var stack = new Stack<char>();
                              foreach (char c in input)
                              {
                                  if (c is '(' or '[' or '{')
                                      stack.Push(c);
                                  else if (c is ')' or ']' or '}')
                                  {
                                      if (stack.Count == 0) return false;
                                      var top = stack.Pop();
                                      if ((c == ')' && top != '(') ||
                                          (c == ']' && top != '[') ||
                                          (c == '}' && top != '{'))
                                          return false;
                                  }
                              }
                              return stack.Count == 0;
                          }
                      }
                      """,
        Explanation:
        "The closing square bracket ']' was being compared against '{' (the opening curly brace) instead of '[' (the opening square bracket). " +
        "This meant that a string like \"[]\" would incorrectly return false because the stack top '[' does not equal '{'. " +
        "Meanwhile, a mixed expression like \"{]\" would pass the check for ']' and only fail later (or not at all), producing incorrect results. " +
        "The fix is to change the condition for ']' to top != '[' so each closing bracket is matched against its correct opener.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class BugHardBracketValidatorTests
                  {
                      [Fact]
                      public void IsBalanced_SquareBrackets_ReturnsTrue()
                      {
                          var sut = new BugHardBracketValidator();
                          Assert.True(sut.IsBalanced("[]"));
                      }

                      [Fact]
                      public void IsBalanced_NestedMixedBrackets_ReturnsTrue()
                      {
                          var sut = new BugHardBracketValidator();
                          Assert.True(sut.IsBalanced("{[()]}"));
                      }

                      [Fact]
                      public void IsBalanced_Parentheses_ReturnsTrue()
                      {
                          var sut = new BugHardBracketValidator();
                          Assert.True(sut.IsBalanced("()"));
                      }

                      [Fact]
                      public void IsBalanced_Mismatched_ReturnsFalse()
                      {
                          var sut = new BugHardBracketValidator();
                          Assert.False(sut.IsBalanced("([)]"));
                      }
                  }
                  """
    );

    private static Template GuessOutput(Difficulties d) => d switch
    {
        Difficulties.Easy => GuessOutputEasy(),
        Difficulties.Medium => GuessOutputMedium(),
        Difficulties.Hard => GuessOutputHard(),
        _ => GuessOutputEasy()
    };

    private static Template GuessOutputEasy() => new(
        ClassName: "GuessOutputEasyFizzBuzz",
        Description:
        "FizzBuzz is a classic categorisation rule used in interview screening tools and code-kata platforms to test conditional-logic skills. " +
        "This method should return \"FizzBuzz\" for multiples of 15, \"Fizz\" for multiples of 3, \"Buzz\" for multiples of 5, and the number itself otherwise. " +
        "The code looks almost correct but contains a logical ordering bug that causes the wrong result to be returned for certain inputs. " +
        "Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Implements the FizzBuzz categorisation rule.
                   /// Used in interview screening tools and code-kata platforms to demonstrate conditional-logic skills.
                   /// </summary>
                   public class GuessOutputEasyFizzBuzz
                   {
                       /// <summary>
                       /// Returns "FizzBuzz" for multiples of 15, "Fizz" for multiples of 3,
                       /// "Buzz" for multiples of 5, and the number as a string for all other values.
                       /// </summary>
                       /// <param name="n">The integer to categorise.</param>
                       /// <returns>A string representation of the FizzBuzz category for <paramref name="n"/>.</returns>
                       // TODO: Something is wrong here
                       public string FizzBuzz(int n)
                       {
                           if (n % 3 == 0) return "Fizz";
                           if (n % 5 == 0) return "Buzz";
                           if (n % 15 == 0) return "FizzBuzz";
                           return n.ToString();
                       }
                   }
                   """,
        Hint: "The order of conditions matters. Which number is divisible by both 3 and 5?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Implements the FizzBuzz categorisation rule.
                      /// Used in interview screening tools and code-kata platforms to demonstrate conditional-logic skills.
                      /// </summary>
                      public class GuessOutputEasyFizzBuzz
                      {
                          /// <summary>
                          /// Returns "FizzBuzz" for multiples of 15, "Fizz" for multiples of 3,
                          /// "Buzz" for multiples of 5, and the number as a string for all other values.
                          /// </summary>
                          /// <param name="n">The integer to categorise.</param>
                          /// <returns>A string representation of the FizzBuzz category for <paramref name="n"/>.</returns>
                          public string FizzBuzz(int n)
                          {
                              if (n % 15 == 0) return "FizzBuzz";
                              if (n % 3 == 0) return "Fizz";
                              if (n % 5 == 0) return "Buzz";
                              return n.ToString();
                          }
                      }
                      """,
        Explanation:
        "Because 15 is divisible by both 3 and 5, the condition n % 3 == 0 matches it first when placed at the top. " +
        "This causes FizzBuzz(15) to return \"Fizz\" instead of \"FizzBuzz\", and the n % 15 branch is never reachable. " +
        "The fix is to check the most specific condition (divisible by 15) before the less specific ones (divisible by 3 or 5), " +
        "ensuring that numbers like 15, 30, and 45 are classified correctly.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class GuessOutputEasyFizzBuzzTests
                  {
                      [Fact]
                      public void FizzBuzz_MultiplOfBoth_ReturnsFizzBuzz()
                      {
                          var sut = new GuessOutputEasyFizzBuzz();
                          Assert.Equal("FizzBuzz", sut.FizzBuzz(15));
                      }

                      [Fact]
                      public void FizzBuzz_MultipleOfThreeOnly_ReturnsFizz()
                      {
                          var sut = new GuessOutputEasyFizzBuzz();
                          Assert.Equal("Fizz", sut.FizzBuzz(9));
                      }

                      [Fact]
                      public void FizzBuzz_MultipleOfFiveOnly_ReturnsBuzz()
                      {
                          var sut = new GuessOutputEasyFizzBuzz();
                          Assert.Equal("Buzz", sut.FizzBuzz(10));
                      }

                      [Fact]
                      public void FizzBuzz_NonMultiple_ReturnsNumber()
                      {
                          var sut = new GuessOutputEasyFizzBuzz();
                          Assert.Equal("7", sut.FizzBuzz(7));
                      }
                  }
                  """
    );

    private static Template GuessOutputMedium() => new(
        ClassName: "GuessOutputMediumStringBuilder",
        Description:
        "Serialization helpers throughout reporting tools and REST APIs rely on comma-separated string construction to format lists for output. " +
        "This method should join a list of strings with a \", \" separator, producing output like \"a, b, c\" with no trailing comma. " +
        "The current implementation introduces an unwanted trailing separator after the last element. " +
        "Identify the flaw and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Text;

                   /// <summary>
                   /// Provides string-joining utilities for serialization and formatting pipelines.
                   /// Used by reporting tools and REST API helpers to produce comma-separated output.
                   /// </summary>
                   public class GuessOutputMediumStringBuilder
                   {
                       /// <summary>
                       /// Joins all items in the list into a single comma-separated string with no trailing separator.
                       /// </summary>
                       /// <param name="items">The list of strings to join.</param>
                       /// <returns>A string of the form "a, b, c", or an empty string if the list is empty.</returns>
                       // TODO: Something is wrong here
                       public string Join(List<string> items)
                       {
                           var sb = new StringBuilder();
                           foreach (var item in items)
                           {
                               sb.Append(item);
                               sb.Append(", ");
                           }
                           return sb.ToString();
                       }
                   }
                   """,
        Hint: "Think about what happens after the last element in the list.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;
                      using System.Text;

                      /// <summary>
                      /// Provides string-joining utilities for serialization and formatting pipelines.
                      /// Used by reporting tools and REST API helpers to produce comma-separated output.
                      /// </summary>
                      public class GuessOutputMediumStringBuilder
                      {
                          /// <summary>
                          /// Joins all items in the list into a single comma-separated string with no trailing separator.
                          /// </summary>
                          /// <param name="items">The list of strings to join.</param>
                          /// <returns>A string of the form "a, b, c", or an empty string if the list is empty.</returns>
                          public string Join(List<string> items)
                          {
                              var sb = new StringBuilder();
                              for (int i = 0; i < items.Count; i++)
                              {
                                  sb.Append(items[i]);
                                  if (i < items.Count - 1)
                                      sb.Append(", ");
                              }
                              return sb.ToString();
                          }
                      }
                      """,
        Explanation:
        "The foreach loop unconditionally appended \", \" after every element, including the last one, producing a trailing separator such as \"a, b, c, \". " +
        "This is a classic off-by-one pattern in string-building loops. " +
        "The fix switches to an index-based for loop and only appends the separator when the current index is not the last, " +
        "so the separator appears exclusively between elements.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class GuessOutputMediumStringBuilderTests
                  {
                      [Fact]
                      public void Join_MultipleItems_NoTrailingComma()
                      {
                          var sut = new GuessOutputMediumStringBuilder();
                          Assert.Equal("a, b, c", sut.Join(["a", "b", "c"]));
                      }

                      [Fact]
                      public void Join_SingleItem_ReturnsItemOnly()
                      {
                          var sut = new GuessOutputMediumStringBuilder();
                          Assert.Equal("only", sut.Join(["only"]));
                      }

                      [Fact]
                      public void Join_EmptyList_ReturnsEmptyString()
                      {
                          var sut = new GuessOutputMediumStringBuilder();
                          Assert.Equal("", sut.Join([]));
                      }
                  }
                  """
    );

    private static Template GuessOutputHard() => new(
        ClassName: "GuessOutputHardClosureCapture",
        Description:
        "Lazy evaluation and deferred execution patterns appear throughout event systems, pipeline builders, and test-data generators in production C# code. " +
        "This method creates a list of Func&lt;int&gt; delegates where each delegate should return the loop index at which it was created (0, 1, 2, …). " +
        "The current implementation falls into the classic C# closure-capture trap — all delegates end up returning the same value. " +
        "Identify why and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Demonstrates deferred-execution patterns using lambda closures.
                   /// Used in pipeline builders and test-data generators where behaviour must be captured at creation time.
                   /// </summary>
                   public class GuessOutputHardClosureCapture
                   {
                       /// <summary>
                       /// Returns a list of delegates where each delegate should return the loop index
                       /// at which it was created (0, 1, 2, … count-1).
                       /// </summary>
                       /// <param name="count">The number of delegates to create.</param>
                       /// <returns>A list of <see cref="Func{Int32}"/> delegates.</returns>
                        // TODO: Something is wrong here – all delegates return the same result
                       public List<Func<int>> CreateMultipliers(int count)
                       {
                           var funcs = new List<Func<int>>();
                           for (int i = 0; i < count; i++)
                           {
                               funcs.Add(() => i);
                           }
                           return funcs;
                       }
                   }
                   """,
        Hint: "What does the lambda capture? Try creating a local copy of i inside the loop.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Demonstrates deferred-execution patterns using lambda closures.
                      /// Used in pipeline builders and test-data generators where behaviour must be captured at creation time.
                      /// </summary>
                      public class GuessOutputHardClosureCapture
                      {
                          /// <summary>
                          /// Returns a list of delegates where each delegate returns the loop index
                          /// at which it was created (0, 1, 2, … count-1).
                          /// </summary>
                          /// <param name="count">The number of delegates to create.</param>
                          /// <returns>A list of <see cref="Func{Int32}"/> delegates.</returns>
                          public List<Func<int>> CreateMultipliers(int count)
                          {
                              var funcs = new List<Func<int>>();
                              for (int i = 0; i < count; i++)
                              {
                                  int captured = i;
                                  funcs.Add(() => captured);
                              }
                              return funcs;
                          }
                      }
                      """,
        Explanation:
        "The lambda () => i captures a reference to the loop variable i, not its value at the point of creation. " +
        "After the loop finishes, i holds the value count, so every delegate in the list returns count when invoked. " +
        "This is the classic C# closure-capture problem with for loops. " +
        "The fix is to introduce a local copy inside the loop body (int captured = i) and have the lambda close over that copy instead. " +
        "Each iteration creates a new variable with its own storage, so each delegate captures an independent value.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class GuessOutputHardClosureCaptureTests
                  {
                      [Fact]
                      public void CreateMultipliers_FirstDelegate_ReturnsZero()
                      {
                          var sut = new GuessOutputHardClosureCapture();
                          var funcs = sut.CreateMultipliers(3);
                          Assert.Equal(0, funcs[0]());
                      }

                      [Fact]
                      public void CreateMultipliers_SecondDelegate_ReturnsOne()
                      {
                          var sut = new GuessOutputHardClosureCapture();
                          var funcs = sut.CreateMultipliers(3);
                          Assert.Equal(1, funcs[1]());
                      }

                      [Fact]
                      public void CreateMultipliers_ThirdDelegate_ReturnsTwo()
                      {
                          var sut = new GuessOutputHardClosureCapture();
                          var funcs = sut.CreateMultipliers(3);
                          Assert.Equal(2, funcs[2]());
                      }
                  }
                  """
    );

    private static Template FillInTheGap(Difficulties d) => d switch
    {
        Difficulties.Easy => FillInTheGapEasy(),
        Difficulties.Medium => FillInTheGapMedium(),
        Difficulties.Hard => FillInTheGapHard(),
        _ => FillInTheGapEasy()
    };

    private static Template FillInTheGapEasy() => new(
        ClassName: "FillInTheGapEasyMaxFinder",
        Description:
        "Utility methods for finding extrema in collections appear in data-processing services, statistics libraries, and dashboard aggregation pipelines. " +
        "This method should scan a list of integers and return the largest value it contains. " +
        "The implementation is intentionally missing — your task is to fill it in without using built-in LINQ helpers like Max(). " +
        "Think about how to track the running maximum as you iterate through the list.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Provides extremum-finding utilities for numeric collections.
                   /// Used by data-processing services and dashboard aggregation pipelines to summarise datasets.
                   /// </summary>
                   public class FillInTheGapEasyMaxFinder
                   {
                       /// <summary>
                       /// Returns the largest integer in the list.
                       /// </summary>
                       /// <param name="numbers">A non-empty list of integers to search.</param>
                       /// <returns>The maximum value found in <paramref name="numbers"/>.</returns>
                        // TODO: Something is wrong here – implement the method
                       public int FindMax(List<int> numbers)
                       {
                           throw new System.NotImplementedException();
                       }
                   }
                   """,
        Hint: "Loop through the list and keep track of the largest value seen so far.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Provides extremum-finding utilities for numeric collections.
                      /// Used by data-processing services and dashboard aggregation pipelines to summarise datasets.
                      /// </summary>
                      public class FillInTheGapEasyMaxFinder
                      {
                          /// <summary>
                          /// Returns the largest integer in the list.
                          /// </summary>
                          /// <param name="numbers">A non-empty list of integers to search.</param>
                          /// <returns>The maximum value found in <paramref name="numbers"/>.</returns>
                          public int FindMax(List<int> numbers)
                          {
                              int max = numbers[0];
                              foreach (var n in numbers)
                                  if (n > max) max = n;
                              return max;
                          }
                      }
                      """,
        Explanation:
        "The algorithm initialises max with the first element so it has a valid baseline drawn from the actual data, avoiding issues with negative numbers that would break an initialisation of int.MinValue unless explicitly handled. " +
        "On each iteration, max is updated whenever a larger value is found. " +
        "After the loop completes, max holds the largest element seen and is returned. " +
        "This is an O(n) linear scan — the simplest correct approach when no sorted structure is available.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class FillInTheGapEasyMaxFinderTests
                  {
                      [Fact]
                      public void FindMax_NormalList_ReturnsLargest()
                      {
                          var sut = new FillInTheGapEasyMaxFinder();
                          Assert.Equal(9, sut.FindMax([3, 1, 9, 4, 6]));
                      }

                      [Fact]
                      public void FindMax_NegativeNumbers_ReturnsLargest()
                      {
                          var sut = new FillInTheGapEasyMaxFinder();
                          Assert.Equal(-1, sut.FindMax([-5, -1, -3]));
                      }

                      [Fact]
                      public void FindMax_SingleElement_ReturnsThatElement()
                      {
                          var sut = new FillInTheGapEasyMaxFinder();
                          Assert.Equal(42, sut.FindMax([42]));
                      }
                  }
                  """
    );

    private static Template FillInTheGapMedium() => new(
        ClassName: "FillInTheGapMediumAnagram",
        Description:
        "Anagram detection is used in word-game engines, plagiarism checkers, and cryptanalysis tools to identify strings that contain the same characters in different arrangements. " +
        "This method should return true if and only if the two input strings are anagrams of each other, using a case-insensitive comparison. " +
        "The implementation body is intentionally missing — implement it without using any external libraries. " +
        "Consider how sorting or frequency counting can reduce the problem to a simple equality check.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Provides anagram-detection logic for text-analysis applications.
                   /// Used in word-game engines, plagiarism checkers, and cryptanalysis tools.
                   /// </summary>
                   public class FillInTheGapMediumAnagram
                   {
                       /// <summary>
                       /// Determines whether two strings are anagrams of each other.
                       /// The comparison is case-insensitive.
                       /// </summary>
                       /// <param name="a">The first string.</param>
                       /// <param name="b">The second string.</param>
                       /// <returns><c>true</c> if <paramref name="a"/> and <paramref name="b"/> are anagrams; otherwise <c>false</c>.</returns>
                        // TODO: Something is wrong here – implement IsAnagram
                       public bool IsAnagram(string a, string b)
                       {
                           throw new System.NotImplementedException();
                       }
                   }
                   """,
        Hint:
        "Two strings are anagrams if they contain exactly the same characters in the same quantities. Think about sorting or frequency counting.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Linq;

                      /// <summary>
                      /// Provides anagram-detection logic for text-analysis applications.
                      /// Used in word-game engines, plagiarism checkers, and cryptanalysis tools.
                      /// </summary>
                      public class FillInTheGapMediumAnagram
                      {
                          /// <summary>
                          /// Determines whether two strings are anagrams of each other.
                          /// The comparison is case-insensitive.
                          /// </summary>
                          /// <param name="a">The first string.</param>
                          /// <param name="b">The second string.</param>
                          /// <returns><c>true</c> if <paramref name="a"/> and <paramref name="b"/> are anagrams; otherwise <c>false</c>.</returns>
                          public bool IsAnagram(string a, string b)
                          {
                              if (a.Length != b.Length) return false;
                              var sortedA = string.Concat(a.ToLower().OrderBy(c => c));
                              var sortedB = string.Concat(b.ToLower().OrderBy(c => c));
                              return sortedA == sortedB;
                          }
                      }
                      """,
        Explanation:
        "The approach is to normalise both strings to lower case, sort their characters, and compare the resulting strings. " +
        "If both sorted forms are identical, the strings contain exactly the same characters in the same quantities, making them anagrams. " +
        "An early-exit length check avoids the O(n log n) sort when the strings differ in length. " +
        "This is a clean O(n log n) solution that avoids needing a mutable frequency dictionary.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class FillInTheGapMediumAnagramTests
                  {
                      [Fact]
                      public void IsAnagram_Anagrams_ReturnsTrue()
                      {
                          var sut = new FillInTheGapMediumAnagram();
                          Assert.True(sut.IsAnagram("listen", "silent"));
                      }

                      [Fact]
                      public void IsAnagram_NotAnagrams_ReturnsFalse()
                      {
                          var sut = new FillInTheGapMediumAnagram();
                          Assert.False(sut.IsAnagram("hello", "world"));
                      }

                      [Fact]
                      public void IsAnagram_DifferentLengths_ReturnsFalse()
                      {
                          var sut = new FillInTheGapMediumAnagram();
                          Assert.False(sut.IsAnagram("abc", "abcd"));
                      }

                      [Fact]
                      public void IsAnagram_CaseInsensitive_ReturnsTrue()
                      {
                          var sut = new FillInTheGapMediumAnagram();
                          Assert.True(sut.IsAnagram("Triangle", "Integral"));
                      }
                  }
                  """
    );

    private static Template FillInTheGapHard() => new(
        ClassName: "FillInTheGapHardLruCache",
        Description:
        "LRU (Least Recently Used) caches are a fundamental building block in web frameworks, database query caches, and CDN edge nodes — wherever fast, bounded memory storage is needed. " +
        "This class should implement a fixed-capacity cache where Get retrieves a value by key (returning -1 if absent) and promotes it to most-recently-used, " +
        "while Put inserts or updates a key-value pair and evicts the least-recently-used entry when the capacity limit is reached. " +
        "Both Get and Put must execute in O(1) amortised time — the data-structure scaffolding is provided; implement the two methods.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Implements a fixed-capacity Least Recently Used (LRU) cache.
                   /// Used in web frameworks, database query caches, and CDN edge nodes for fast, bounded in-memory storage.
                   /// Both Get and Put run in O(1) amortised time.
                   /// </summary>
                   public class FillInTheGapHardLruCache
                   {
                       private readonly int _capacity;
                       private readonly Dictionary<int, LinkedListNode<(int key, int value)>> _map;
                       private readonly LinkedList<(int key, int value)> _list;

                       /// <summary>
                       /// Initialises a new LRU cache with the specified maximum capacity.
                       /// </summary>
                       /// <param name="capacity">The maximum number of entries the cache may hold.</param>
                       public FillInTheGapHardLruCache(int capacity)
                       {
                           _capacity = capacity;
                           _map = new Dictionary<int, LinkedListNode<(int key, int value)>>();
                           _list = new LinkedList<(int key, int value)>();
                       }

                       /// <summary>
                       /// Retrieves the value associated with the key and marks it as most recently used.
                       /// </summary>
                       /// <param name="key">The cache key to look up.</param>
                       /// <returns>The cached value, or -1 if the key is not present.</returns>
                        // TODO: Something is wrong here – implement Get and Put
                       public int Get(int key)
                       {
                           throw new System.NotImplementedException();
                       }

                       /// <summary>
                       /// Inserts or updates the key-value pair and evicts the least recently used entry if capacity is exceeded.
                       /// </summary>
                       /// <param name="key">The cache key.</param>
                       /// <param name="value">The value to store.</param>
                       public void Put(int key, int value)
                       {
                           throw new System.NotImplementedException();
                       }
                   }
                   """,
        Hint:
        "Use a LinkedList for ordering and a Dictionary for O(1) lookups. Get should move the node to the front. Put should evict the last entry in the list when capacity is reached.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Implements a fixed-capacity Least Recently Used (LRU) cache.
                      /// Used in web frameworks, database query caches, and CDN edge nodes for fast, bounded in-memory storage.
                      /// Both Get and Put run in O(1) amortised time.
                      /// </summary>
                      public class FillInTheGapHardLruCache
                      {
                          private readonly int _capacity;
                          private readonly Dictionary<int, LinkedListNode<(int key, int value)>> _map;
                          private readonly LinkedList<(int key, int value)> _list;

                          /// <summary>
                          /// Initialises a new LRU cache with the specified maximum capacity.
                          /// </summary>
                          /// <param name="capacity">The maximum number of entries the cache may hold.</param>
                          public FillInTheGapHardLruCache(int capacity)
                          {
                              _capacity = capacity;
                              _map = new Dictionary<int, LinkedListNode<(int key, int value)>>();
                              _list = new LinkedList<(int key, int value)>();
                          }

                          /// <summary>
                          /// Retrieves the value associated with the key and marks it as most recently used.
                          /// </summary>
                          /// <param name="key">The cache key to look up.</param>
                          /// <returns>The cached value, or -1 if the key is not present.</returns>
                          public int Get(int key)
                          {
                              if (!_map.TryGetValue(key, out var node)) return -1;
                              _list.Remove(node);
                              _list.AddFirst(node);
                              return node.Value.value;
                          }

                          /// <summary>
                          /// Inserts or updates the key-value pair and evicts the least recently used entry if capacity is exceeded.
                          /// </summary>
                          /// <param name="key">The cache key.</param>
                          /// <param name="value">The value to store.</param>
                          public void Put(int key, int value)
                          {
                              if (_map.TryGetValue(key, out var existing))
                              {
                                  _list.Remove(existing);
                                  _map.Remove(key);
                              }
                              else if (_map.Count == _capacity)
                              {
                                  var lru = _list.Last!;
                                  _map.Remove(lru.Value.key);
                                  _list.RemoveLast();
                              }
                              var node = _list.AddFirst((key, value));
                              _map[key] = node;
                          }
                      }
                      """,
        Explanation:
        "The dual-structure design is the key insight: the LinkedList maintains access order (most recently used at the front, least recently used at the back) while the Dictionary provides O(1) key-to-node lookups. " +
        "Get moves the found node to the front of the list to record the access, then returns its value. " +
        "Put first removes any existing node for the key to avoid duplicates, then evicts the tail node when the cache is full before inserting the new node at the front. " +
        "Both operations touch a constant number of nodes regardless of cache size, giving the required O(1) time complexity.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class FillInTheGapHardLruCacheTests
                  {
                      [Fact]
                      public void Get_ExistingKey_ReturnsValue()
                      {
                          var cache = new FillInTheGapHardLruCache(2);
                          cache.Put(1, 10);
                          Assert.Equal(10, cache.Get(1));
                      }

                      [Fact]
                      public void Get_MissingKey_ReturnsMinusOne()
                      {
                          var cache = new FillInTheGapHardLruCache(2);
                          Assert.Equal(-1, cache.Get(99));
                      }

                      [Fact]
                      public void Put_ExceedsCapacity_EvictsLeastRecentlyUsed()
                      {
                          var cache = new FillInTheGapHardLruCache(2);
                          cache.Put(1, 1);
                          cache.Put(2, 2);
                          cache.Get(1);
                          cache.Put(3, 3);
                          Assert.Equal(-1, cache.Get(2));
                          Assert.Equal(1, cache.Get(1));
                          Assert.Equal(3, cache.Get(3));
                      }
                  }
                  """
    );

    private static Template AlgorithmRiddle(Difficulties d) => d switch
    {
        Difficulties.Easy => AlgorithmRiddleEasy(),
        Difficulties.Medium => AlgorithmRiddleMedium(),
        Difficulties.Hard => AlgorithmRiddleHard(),
        _ => AlgorithmRiddleEasy()
    };

    private static Template AlgorithmRiddleEasy() => new(
        ClassName: "AlgorithmRiddleEasyIsPrime",
        Description:
        "Prime-number checks are used in cryptographic key generation, hash-table sizing algorithms, and number-theory utilities throughout security and data-structure libraries. " +
        "This method should return true if a given integer is a prime number and false otherwise, handling edge cases such as numbers less than 2. " +
        "The implementation body is intentionally missing — implement it efficiently without using any external libraries. " +
        "Consider the mathematical definition of a prime and how far you actually need to iterate.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Provides primality-testing utilities for number-theory and cryptographic applications.
                   /// Used in cryptographic key-generation libraries and hash-table sizing algorithms.
                   /// </summary>
                   public class AlgorithmRiddleEasyIsPrime
                   {
                       /// <summary>
                       /// Determines whether the specified integer is a prime number.
                       /// </summary>
                       /// <param name="n">The integer to test. Values less than 2 are not prime.</param>
                       /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise <c>false</c>.</returns>
                       // TODO: Something is wrong here – implement IsPrime
                       public bool IsPrime(int n)
                       {
                           throw new System.NotImplementedException();
                       }
                   }
                   """,
        Hint: "A prime is only divisible by 1 and itself. Check divisors from 2 up to √n.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Provides primality-testing utilities for number-theory and cryptographic applications.
                      /// Used in cryptographic key-generation libraries and hash-table sizing algorithms.
                      /// </summary>
                      public class AlgorithmRiddleEasyIsPrime
                      {
                          /// <summary>
                          /// Determines whether the specified integer is a prime number.
                          /// </summary>
                          /// <param name="n">The integer to test. Values less than 2 are not prime.</param>
                          /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise <c>false</c>.</returns>
                          public bool IsPrime(int n)
                          {
                              if (n < 2) return false;
                              for (int i = 2; i * i <= n; i++)
                                  if (n % i == 0) return false;
                              return true;
                          }
                      }
                      """,
        Explanation:
        "The key insight is that if n has a factor greater than √n, it must also have a corresponding factor smaller than √n, " +
        "so iterating only up to √n (expressed as i * i <= n to avoid floating-point) is sufficient. " +
        "Numbers less than 2 are handled by the early return since they do not satisfy the prime definition. " +
        "If the loop completes without finding a divisor, n is prime and the method returns true.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class AlgorithmRiddleEasyIsPrimeTests
                  {
                      [Fact]
                      public void IsPrime_PrimeNumber_ReturnsTrue()
                      {
                          var sut = new AlgorithmRiddleEasyIsPrime();
                          Assert.True(sut.IsPrime(7));
                      }

                      [Fact]
                      public void IsPrime_CompositeNumber_ReturnsFalse()
                      {
                          var sut = new AlgorithmRiddleEasyIsPrime();
                          Assert.False(sut.IsPrime(9));
                      }

                      [Fact]
                      public void IsPrime_One_ReturnsFalse()
                      {
                          var sut = new AlgorithmRiddleEasyIsPrime();
                          Assert.False(sut.IsPrime(1));
                      }

                      [Fact]
                      public void IsPrime_Two_ReturnsTrue()
                      {
                          var sut = new AlgorithmRiddleEasyIsPrime();
                          Assert.True(sut.IsPrime(2));
                      }
                  }
                  """
    );

    private static Template AlgorithmRiddleMedium() => new(
        ClassName: "AlgorithmRiddleMediumDigitSum",
        Description:
        "Digit-sum computation is used in checksum algorithms, ISBN/EAN validation, and numerology-based data-integrity checks across billing and logistics systems. " +
        "This method should recursively compute the sum of all decimal digits in a number, correctly handling negative inputs by treating them as their absolute value. " +
        "The current implementation contains a subtle recursive-call bug that causes it to return incorrect results for any number with more than one digit. " +
        "Find and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Computes the digit sum of an integer using recursion.
                   /// Used in checksum algorithms, ISBN validation, and data-integrity checks in billing systems.
                   /// </summary>
                   public class AlgorithmRiddleMediumDigitSum
                   {
                       /// <summary>
                       /// Recursively sums all decimal digits of the specified number.
                       /// Negative values are treated as their absolute equivalent.
                       /// </summary>
                       /// <param name="n">The integer whose digits are to be summed.</param>
                       /// <returns>The sum of all decimal digits of <paramref name="n"/>.</returns>
                       // TODO: Something is wrong here
                       public int SumDigits(int n)
                       {
                           n = System.Math.Abs(n);
                           if (n < 10) return n;
                           return (n % 10) + SumDigits(n % 10);
                       }
                   }
                   """,
        Hint: "Think about what 'n / 10' versus 'n % 10' gives you when recursing to the next digit.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Computes the digit sum of an integer using recursion.
                      /// Used in checksum algorithms, ISBN validation, and data-integrity checks in billing systems.
                      /// </summary>
                      public class AlgorithmRiddleMediumDigitSum
                      {
                          /// <summary>
                          /// Recursively sums all decimal digits of the specified number.
                          /// Negative values are treated as their absolute equivalent.
                          /// </summary>
                          /// <param name="n">The integer whose digits are to be summed.</param>
                          /// <returns>The sum of all decimal digits of <paramref name="n"/>.</returns>
                          public int SumDigits(int n)
                          {
                              n = System.Math.Abs(n);
                              if (n < 10) return n;
                              return (n % 10) + SumDigits(n / 10);
                          }
                      }
                      """,
        Explanation:
        "The recursive call passed n % 10 (the last digit) instead of n / 10 (the number with the last digit removed). " +
        "This meant the recursion never advanced through the number — it repeatedly extracted the same last digit and added it to itself, producing wildly incorrect totals for multi-digit numbers. " +
        "The correct recursive step is n / 10, which strips the last digit and reduces the problem by one digit on each call. " +
        "The base case (n < 10) terminates the recursion once a single digit remains.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class AlgorithmRiddleMediumDigitSumTests
                  {
                      [Fact]
                      public void SumDigits_ThreeDigitNumber_ReturnsCorrectSum()
                      {
                          var sut = new AlgorithmRiddleMediumDigitSum();
                          Assert.Equal(15, sut.SumDigits(456));
                      }

                      [Fact]
                      public void SumDigits_SingleDigit_ReturnsSelf()
                      {
                          var sut = new AlgorithmRiddleMediumDigitSum();
                          Assert.Equal(7, sut.SumDigits(7));
                      }

                      [Fact]
                      public void SumDigits_TwoDigitNumber_ReturnsCorrectSum()
                      {
                          var sut = new AlgorithmRiddleMediumDigitSum();
                          Assert.Equal(1, sut.SumDigits(10));
                      }

                      [Fact]
                      public void SumDigits_NegativeNumber_ReturnsPositiveSum()
                      {
                          var sut = new AlgorithmRiddleMediumDigitSum();
                          Assert.Equal(6, sut.SumDigits(-123));
                      }
                  }
                  """
    );

    private static Template AlgorithmRiddleHard() => new(
        ClassName: "AlgorithmRiddleHardMaxSubarray",
        Description:
        "Maximum-subarray problems appear in financial analytics (finding the best buy-sell window), signal processing (identifying peak regions), and game-score analysis. " +
        "This method should find the contiguous subarray with the largest sum using Kadane's algorithm, which runs in O(n). " +
        "The current implementation updates the global maximum with the wrong variable, causing incorrect results on arrays with mixed positive and negative values. " +
        "Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Finds the maximum-sum contiguous subarray using Kadane's algorithm.
                   /// Used in financial analytics to identify best buy-sell windows and in signal processing to locate peak regions.
                   /// </summary>
                   public class AlgorithmRiddleHardMaxSubarray
                   {
                       /// <summary>
                       /// Returns the sum of the contiguous subarray with the largest total within <paramref name="nums"/>.
                       /// Handles arrays containing negative numbers; always returns the single largest element if all values are negative.
                       /// </summary>
                       /// <param name="nums">A non-empty array of integers.</param>
                       /// <returns>The maximum subarray sum.</returns>
                       // TODO: Something is wrong here
                       public int MaxSubarraySum(int[] nums)
                       {
                           int maxSoFar = nums[0];
                           int maxEndingHere = nums[0];
                           for (int i = 1; i < nums.Length; i++)
                           {
                               maxEndingHere = System.Math.Max(nums[i], maxEndingHere + nums[i]);
                               maxSoFar = System.Math.Max(maxSoFar, nums[i]);
                           }
                           return maxSoFar;
                       }
                   }
                   """,
        Hint: "After extending or starting a new subarray, what should you compare against the global maximum?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Finds the maximum-sum contiguous subarray using Kadane's algorithm.
                      /// Used in financial analytics to identify best buy-sell windows and in signal processing to locate peak regions.
                      /// </summary>
                      public class AlgorithmRiddleHardMaxSubarray
                      {
                          /// <summary>
                          /// Returns the sum of the contiguous subarray with the largest total within <paramref name="nums"/>.
                          /// Handles arrays containing negative numbers; always returns the single largest element if all values are negative.
                          /// </summary>
                          /// <param name="nums">A non-empty array of integers.</param>
                          /// <returns>The maximum subarray sum.</returns>
                          public int MaxSubarraySum(int[] nums)
                          {
                              int maxSoFar = nums[0];
                              int maxEndingHere = nums[0];
                              for (int i = 1; i < nums.Length; i++)
                              {
                                  maxEndingHere = System.Math.Max(nums[i], maxEndingHere + nums[i]);
                                  maxSoFar = System.Math.Max(maxSoFar, maxEndingHere);
                              }
                              return maxSoFar;
                          }
                      }
                      """,
        Explanation:
        "The bug was that maxSoFar was updated with nums[i] — the current single element — instead of maxEndingHere, which holds the best subarray sum ending at position i. " +
        "This meant the global maximum only ever considered individual elements, ignoring multi-element subarrays with larger combined sums. " +
        "For example, on [-2, 1, -3, 4, -1, 2, 1, -5, 4] the correct answer is 6 (subarray [4,-1,2,1]), but the bug returns 4 (the single largest element). " +
        "The fix is to compare maxSoFar against maxEndingHere so the running best-subarray sum is tracked correctly at every step.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class AlgorithmRiddleHardMaxSubarrayTests
                  {
                      [Fact]
                      public void MaxSubarraySum_MixedValues_ReturnsCorrectSum()
                      {
                          var sut = new AlgorithmRiddleHardMaxSubarray();
                          Assert.Equal(6, sut.MaxSubarraySum([-2, 1, -3, 4, -1, 2, 1, -5, 4]));
                      }

                      [Fact]
                      public void MaxSubarraySum_AllNegative_ReturnsLeastNegative()
                      {
                          var sut = new AlgorithmRiddleHardMaxSubarray();
                          Assert.Equal(-1, sut.MaxSubarraySum([-1, -2, -3]));
                      }

                      [Fact]
                      public void MaxSubarraySum_SingleElement_ReturnsThatElement()
                      {
                          var sut = new AlgorithmRiddleHardMaxSubarray();
                          Assert.Equal(5, sut.MaxSubarraySum([5]));
                      }

                      [Fact]
                      public void MaxSubarraySum_AllPositive_ReturnsTotalSum()
                      {
                          var sut = new AlgorithmRiddleHardMaxSubarray();
                          Assert.Equal(10, sut.MaxSubarraySum([1, 2, 3, 4]));
                      }
                  }
                  """
    );

    private static Template Linq(Difficulties d) => d switch
    {
        Difficulties.Easy => LinqEasy(),
        Difficulties.Medium => LinqMedium(),
        Difficulties.Hard => LinqHard(),
        _ => LinqEasy()
    };

    private static Template LinqEasy() => new(
        ClassName: "LinqEasyEvenFilter",
        Description:
        "Filtering collections by numeric properties is a staple operation in data-processing layers of e-commerce, finance, and analytics applications — for example, selecting only even-numbered order IDs for batch processing. " +
        "This method should use a LINQ Where clause to return only the even numbers from a list of integers. " +
        "The current predicate contains a logical error that causes it to return the wrong subset. " +
        "Fix the LINQ query.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Linq;

                   /// <summary>
                   /// Provides LINQ-based filtering utilities for numeric collections.
                   /// Used in data-processing layers of e-commerce and analytics applications to select subsets based on numeric properties.
                   /// </summary>
                   public class LinqEasyEvenFilter
                   {
                       /// <summary>
                       /// Returns a new list containing only the even numbers from the source list.
                       /// </summary>
                       /// <param name="numbers">The source list of integers to filter.</param>
                       /// <returns>A list of even integers in the same order they appeared in <paramref name="numbers"/>.</returns>
                       // TODO: Something is wrong here
                       public List<int> GetEvens(List<int> numbers)
                       {
                           return numbers.Where(x => x % 2 != 0).ToList();
                       }
                   }
                   """,
        Hint: "Check the modulo condition — what remainder does an even number leave when divided by 2?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;
                      using System.Linq;

                      /// <summary>
                      /// Provides LINQ-based filtering utilities for numeric collections.
                      /// Used in data-processing layers of e-commerce and analytics applications to select subsets based on numeric properties.
                      /// </summary>
                      public class LinqEasyEvenFilter
                      {
                          /// <summary>
                          /// Returns a new list containing only the even numbers from the source list.
                          /// </summary>
                          /// <param name="numbers">The source list of integers to filter.</param>
                          /// <returns>A list of even integers in the same order they appeared in <paramref name="numbers"/>.</returns>
                          public List<int> GetEvens(List<int> numbers)
                          {
                              return numbers.Where(x => x % 2 == 0).ToList();
                          }
                      }
                      """,
        Explanation:
        "The predicate x % 2 != 0 is true when the remainder is 1, meaning it keeps odd numbers and discards even ones — the exact opposite of what is required. " +
        "Even numbers leave a remainder of 0 when divided by 2, so the correct condition is x % 2 == 0. " +
        "This is a common sign-flip mistake when writing filter predicates; always verify the condition by mentally substituting a known value (e.g. 4 % 2 == 0 → true, keep it).",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class LinqEasyEvenFilterTests
                  {
                      [Fact]
                      public void GetEvens_MixedList_ReturnsOnlyEvens()
                      {
                          var sut = new LinqEasyEvenFilter();
                          Assert.Equal([2, 4], sut.GetEvens([1, 2, 3, 4, 5]));
                      }

                      [Fact]
                      public void GetEvens_AllEven_ReturnsAll()
                      {
                          var sut = new LinqEasyEvenFilter();
                          Assert.Equal([2, 4, 6], sut.GetEvens([2, 4, 6]));
                      }

                      [Fact]
                      public void GetEvens_AllOdd_ReturnsEmpty()
                      {
                          var sut = new LinqEasyEvenFilter();
                          Assert.Empty(sut.GetEvens([1, 3, 5]));
                      }
                  }
                  """
    );

    private static Template LinqMedium() => new(
        ClassName: "LinqMediumTopScorers",
        Description:
        "Leaderboard APIs in gaming platforms, e-learning systems, and sales dashboards routinely need to surface the top-N performers from a collection of scored records. " +
        "This method should return the names of the three highest-scoring players by chaining OrderBy, Take, and Select in a LINQ pipeline. " +
        "The current implementation sorts in the wrong direction and therefore returns the bottom scorers instead. " +
        "Fix the LINQ query.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Linq;

                   /// <summary>
                   /// Provides leaderboard-query utilities for scored-record collections.
                   /// Used by gaming platforms, e-learning systems, and sales dashboards to surface top-N performers.
                   /// </summary>
                   public class LinqMediumTopScorers
                   {
                       /// <summary>
                       /// Returns the names of the three highest-scoring players in descending score order.
                       /// </summary>
                       /// <param name="players">A list of (Name, Score) tuples representing the player roster.</param>
                       /// <returns>A list of up to 3 player names, ordered from highest to lowest score.</returns>
                       // TODO: Something is wrong here
                       public List<string> GetTopThree(List<(string Name, int Score)> players)
                       {
                           return players
                               .OrderBy(p => p.Score)
                               .Take(3)
                               .Select(p => p.Name)
                               .ToList();
                       }
                   }
                   """,
        Hint: "Look at the ordering direction — do you want the highest or lowest scores appearing first?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;
                      using System.Linq;

                      /// <summary>
                      /// Provides leaderboard-query utilities for scored-record collections.
                      /// Used by gaming platforms, e-learning systems, and sales dashboards to surface top-N performers.
                      /// </summary>
                      public class LinqMediumTopScorers
                      {
                          /// <summary>
                          /// Returns the names of the three highest-scoring players in descending score order.
                          /// </summary>
                          /// <param name="players">A list of (Name, Score) tuples representing the player roster.</param>
                          /// <returns>A list of up to 3 player names, ordered from highest to lowest score.</returns>
                          public List<string> GetTopThree(List<(string Name, int Score)> players)
                          {
                              return players
                                  .OrderByDescending(p => p.Score)
                                  .Take(3)
                                  .Select(p => p.Name)
                                  .ToList();
                          }
                      }
                      """,
        Explanation:
        "OrderBy sorts in ascending order (lowest score first), so Take(3) picks the three worst performers — the exact opposite of a top-scorer leaderboard. " +
        "Replacing OrderBy with OrderByDescending ensures the highest scores appear first, making Take(3) select the three best players. " +
        "This is a common LINQ mistake: when retrieving top-N items you almost always want descending order before the Take.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class LinqMediumTopScorersTests
                  {
                      [Fact]
                      public void GetTopThree_ContainsHighestScorer()
                      {
                          var sut = new LinqMediumTopScorers();
                          var players = new System.Collections.Generic.List<(string, int)>
                          {
                              ("Alice", 80), ("Bob", 95), ("Charlie", 70), ("Diana", 88)
                          };
                          var result = sut.GetTopThree(players);
                          Assert.Contains("Bob", result);
                      }

                      [Fact]
                      public void GetTopThree_ExcludesLowestScorer()
                      {
                          var sut = new LinqMediumTopScorers();
                          var players = new System.Collections.Generic.List<(string, int)>
                          {
                              ("Alice", 80), ("Bob", 95), ("Charlie", 70), ("Diana", 88)
                          };
                          var result = sut.GetTopThree(players);
                          Assert.DoesNotContain("Charlie", result);
                      }

                      [Fact]
                      public void GetTopThree_ReturnsExactlyThree()
                      {
                          var sut = new LinqMediumTopScorers();
                          var players = new System.Collections.Generic.List<(string, int)>
                          {
                              ("A", 10), ("B", 20), ("C", 30), ("D", 40)
                          };
                          Assert.Equal(3, sut.GetTopThree(players).Count);
                      }
                  }
                  """
    );

    private static Template LinqHard() => new(
        ClassName: "LinqHardGroupAverage",
        Description:
        "Aggregated pricing reports are a standard requirement in product-catalogue services, ERP systems, and retail analytics platforms — for example, calculating the mean price per product category to support purchasing decisions. " +
        "This method should use LINQ GroupBy and ToDictionary to compute the average price of all products in each category, returning a dictionary keyed by category name. " +
        "The current implementation groups by the wrong field, producing one entry per product instead of one entry per category. " +
        "Fix the LINQ query.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Linq;

                   /// <summary>
                   /// Provides category-level price aggregation for product-catalogue services.
                   /// Used by ERP systems and retail analytics platforms to compute average prices per category.
                   /// </summary>
                   public class LinqHardGroupAverage
                   {
                       /// <summary>
                       /// Computes the average price of products grouped by their category.
                       /// </summary>
                       /// <param name="products">
                       /// A list of (Category, Name, Price) tuples representing the product catalogue.
                       /// </param>
                       /// <returns>
                       /// A dictionary mapping each distinct category name to the average price of products in that category.
                       /// </returns>
                       // TODO: Something is wrong here
                       public Dictionary<string, double> AverageByCategory(
                           List<(string Category, string Name, double Price)> products)
                       {
                           return products
                               .GroupBy(p => p.Name)
                               .ToDictionary(g => g.Key, g => g.Average(p => p.Price));
                       }
                   }
                   """,
        Hint: "Which field uniquely identifies a category rather than an individual product?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;
                      using System.Linq;

                      /// <summary>
                      /// Provides category-level price aggregation for product-catalogue services.
                      /// Used by ERP systems and retail analytics platforms to compute average prices per category.
                      /// </summary>
                      public class LinqHardGroupAverage
                      {
                          /// <summary>
                          /// Computes the average price of products grouped by their category.
                          /// </summary>
                          /// <param name="products">
                          /// A list of (Category, Name, Price) tuples representing the product catalogue.
                          /// </param>
                          /// <returns>
                          /// A dictionary mapping each distinct category name to the average price of products in that category.
                          /// </returns>
                          public Dictionary<string, double> AverageByCategory(
                              List<(string Category, string Name, double Price)> products)
                          {
                              return products
                                  .GroupBy(p => p.Category)
                                  .ToDictionary(g => g.Key, g => g.Average(p => p.Price));
                          }
                      }
                      """,
        Explanation:
        "GroupBy(p => p.Name) groups by product name, which creates one group per individual product since product names are unique. " +
        "This makes the subsequent Average meaningless (a group of one always averages to itself) and produces a dictionary with as many entries as products rather than categories. " +
        "The correct key is p.Category, which groups all products sharing the same category into a single group so the Average is computed across multiple prices. " +
        "When writing GroupBy, always ask: 'what should define membership of a group?' — here the answer is the category, not the product name.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class LinqHardGroupAverageTests
                  {
                      [Fact]
                      public void AverageByCategory_ReturnsCorrectCategoryKeys()
                      {
                          var sut = new LinqHardGroupAverage();
                          var products = new System.Collections.Generic.List<(string, string, double)>
                          {
                              ("Electronics", "Phone", 500), ("Electronics", "Laptop", 1000),
                              ("Food", "Apple", 1), ("Food", "Bread", 2)
                          };
                          var result = sut.AverageByCategory(products);
                          Assert.True(result.ContainsKey("Electronics"));
                          Assert.True(result.ContainsKey("Food"));
                      }

                      [Fact]
                      public void AverageByCategory_ElectronicsAverage_IsCorrect()
                      {
                          var sut = new LinqHardGroupAverage();
                          var products = new System.Collections.Generic.List<(string, string, double)>
                          {
                              ("Electronics", "Phone", 500), ("Electronics", "Laptop", 1000)
                          };
                          var result = sut.AverageByCategory(products);
                          Assert.Equal(750.0, result["Electronics"]);
                      }

                      [Fact]
                      public void AverageByCategory_TwoCategories_ReturnsTwoEntries()
                      {
                          var sut = new LinqHardGroupAverage();
                          var products = new System.Collections.Generic.List<(string, string, double)>
                          {
                              ("A", "x", 10), ("A", "y", 20), ("B", "z", 30)
                          };
                          Assert.Equal(2, sut.AverageByCategory(products).Count);
                      }
                  }
                  """
    );

    private static Template BlackBox(Difficulties d) => d switch
    {
        Difficulties.Easy => BlackBoxEasy(),
        Difficulties.Medium => BlackBoxMedium(),
        Difficulties.Hard => BlackBoxHard(),
        _ => BlackBoxEasy()
    };

    private static Template BlackBoxEasy() => new(
        ClassName: "BlackBoxEasyTransformer",
        Description:
        "In input-sanitization pipelines and numeric display formatters, it is common to guarantee that a value is non-negative before rendering or storing it. " +
        "The tests below describe the exact contract that Transform must satisfy — read them carefully to infer what the method should do, then identify and fix the bug in the implementation. " +
        "No documentation is provided intentionally; the tests are your specification.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Performs a numeric transformation used in input-sanitization and display-formatting pipelines.
                   /// Derive the exact contract from the unit tests.
                   /// </summary>
                   public class BlackBoxEasyTransformer
                   {
                       /// <summary>
                       /// Transforms the integer according to the contract described by the unit tests.
                       /// </summary>
                       /// <param name="n">The integer to transform.</param>
                       /// <returns>The transformed value.</returns>
                       // TODO: Something is wrong here – figure out the intent from the tests
                       public int Transform(int n)
                       {
                           if (n < 0) return -n;
                           return -n;
                       }
                   }
                   """,
        Hint: "Run the tests mentally — what does the method return for positive numbers? What about negative ones?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Performs a numeric transformation used in input-sanitization and display-formatting pipelines.
                      /// Derive the exact contract from the unit tests.
                      /// </summary>
                      public class BlackBoxEasyTransformer
                      {
                          /// <summary>
                          /// Transforms the integer according to the contract described by the unit tests.
                          /// </summary>
                          /// <param name="n">The integer to transform.</param>
                          /// <returns>The transformed value.</returns>
                          public int Transform(int n)
                          {
                              if (n < 0) return -n;
                              return n;
                          }
                      }
                      """,
        Explanation:
        "Reading the tests reveals that Transform is Math.Abs — it should return the absolute value of the input, " +
        "making negative numbers positive and leaving non-negative numbers unchanged. " +
        "The bug is in the positive branch: it returns -n, which negates a positive number rather than leaving it as-is. " +
        "The negative branch (return -n for n < 0) is correct. " +
        "The fix is to change the positive branch to return n.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class BlackBoxEasyTransformerTests
                  {
                      [Fact]
                      public void Transform_NegativeInput_ReturnsPositive()
                      {
                          var sut = new BlackBoxEasyTransformer();
                          Assert.Equal(5, sut.Transform(-5));
                      }

                      [Fact]
                      public void Transform_PositiveInput_ReturnsSameValue()
                      {
                          var sut = new BlackBoxEasyTransformer();
                          Assert.Equal(5, sut.Transform(5));
                      }

                      [Fact]
                      public void Transform_Zero_ReturnsZero()
                      {
                          var sut = new BlackBoxEasyTransformer();
                          Assert.Equal(0, sut.Transform(0));
                      }
                  }
                  """
    );

    private static Template BlackBoxMedium() => new(
        ClassName: "BlackBoxMediumListShifter",
        Description:
        "Circular-shift operations appear in ring-buffer implementations, scheduler round-robin algorithms, and cryptographic permutation steps. " +
        "The tests below define the exact behaviour of Shift — analyse them to determine the direction and magnitude of the rotation, then identify and fix the bug in the implementation. " +
        "No documentation is provided intentionally; derive the contract entirely from the tests.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Performs circular-shift (rotation) operations on integer lists.
                   /// Used in ring-buffer implementations, round-robin schedulers, and cryptographic permutation steps.
                   /// Derive the exact contract from the unit tests.
                   /// </summary>
                   public class BlackBoxMediumListShifter
                   {
                       /// <summary>
                       /// Shifts the list by <paramref name="k"/> positions according to the contract described by the unit tests.
                       /// </summary>
                       /// <param name="items">The list to shift.</param>
                       /// <param name="k">The number of positions to shift by.</param>
                       /// <returns>A new list with elements shifted by <paramref name="k"/> positions.</returns>
                       // TODO: Something is wrong here – figure out the intent from the tests
                       public List<int> Shift(List<int> items, int k)
                       {
                           if (items.Count == 0) return items;
                           k = k % items.Count;
                           var result = new List<int>();
                           result.AddRange(items.GetRange(k, items.Count - k));
                           result.AddRange(items.GetRange(0, k));
                           return result;
                       }
                   }
                   """,
        Hint: "Trace through the tests step by step — which end of the list does the shift take elements from?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Performs circular-shift (rotation) operations on integer lists.
                      /// Used in ring-buffer implementations, round-robin schedulers, and cryptographic permutation steps.
                      /// Derive the exact contract from the unit tests.
                      /// </summary>
                      public class BlackBoxMediumListShifter
                      {
                          /// <summary>
                          /// Shifts the list by <paramref name="k"/> positions according to the contract described by the unit tests.
                          /// </summary>
                          /// <param name="items">The list to shift.</param>
                          /// <param name="k">The number of positions to shift by.</param>
                          /// <returns>A new list with elements shifted by <paramref name="k"/> positions.</returns>
                          public List<int> Shift(List<int> items, int k)
                          {
                              if (items.Count == 0) return items;
                              k = k % items.Count;
                              var result = new List<int>();
                              result.AddRange(items.GetRange(items.Count - k, k));
                              result.AddRange(items.GetRange(0, items.Count - k));
                              return result;
                          }
                      }
                      """,
        Explanation:
        "The tests reveal that Shift rotates the list rightward by k positions: the last k elements move to the front and the remaining elements follow. " +
        "The buggy implementation uses GetRange(k, ...) and GetRange(0, k), which takes elements from the front — that is a left rotation, the opposite direction. " +
        "The fix is to take the last k elements first (GetRange(items.Count - k, k)) to form the new head, followed by the first items.Count - k elements as the tail. " +
        "The modulo on k handles values larger than the list length gracefully.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class BlackBoxMediumListShifterTests
                  {
                      [Fact]
                      public void Shift_ByTwo_LastTwoElementsMoveToFront()
                      {
                          var sut = new BlackBoxMediumListShifter();
                          Assert.Equal([4, 5, 1, 2, 3], sut.Shift([1, 2, 3, 4, 5], 2));
                      }

                      [Fact]
                      public void Shift_ByOne_LastElementMovesToFront()
                      {
                          var sut = new BlackBoxMediumListShifter();
                          Assert.Equal([3, 1, 2], sut.Shift([1, 2, 3], 1));
                      }

                      [Fact]
                      public void Shift_EmptyList_ReturnsEmpty()
                      {
                          var sut = new BlackBoxMediumListShifter();
                          Assert.Empty(sut.Shift([], 5));
                      }
                  }
                  """
    );

    private static Template BlackBoxHard() => new(
        ClassName: "BlackBoxHardRunLengthEncoder",
        Description:
        "Run-length encoding (RLE) is a lossless compression technique used in fax transmission, BMP image formats, and simple protocol framing to reduce repeated sequences of identical bytes. " +
        "The tests below define the exact encoding contract for Encode — study them carefully to determine the output format, then identify and fix the subtle counter-reset bug in the implementation. " +
        "No documentation is provided intentionally; the tests are your specification.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Text;

                   /// <summary>
                   /// Implements run-length encoding (RLE) for string compression.
                   /// Used in fax transmission, BMP image codecs, and protocol framing to compress repeated character sequences.
                   /// Derive the exact encoding contract from the unit tests.
                   /// </summary>
                   public class BlackBoxHardRunLengthEncoder
                   {
                       /// <summary>
                       /// Encodes the input string using run-length encoding according to the contract described by the unit tests.
                       /// </summary>
                       /// <param name="input">The string to encode.</param>
                       /// <returns>The run-length encoded representation of <paramref name="input"/>.</returns>
                       // TODO: Something is wrong here – figure out the intent from the tests
                       public string Encode(string input)
                       {
                           if (string.IsNullOrEmpty(input)) return "";
                           var sb = new StringBuilder();
                           int count = 1;
                           for (int i = 1; i <= input.Length; i++)
                           {
                               if (i < input.Length && input[i] == input[i - 1])
                               {
                                   count++;
                               }
                               else
                               {
                                   sb.Append(count);
                                   sb.Append(input[i - 1]);
                                   count = 0;
                               }
                           }
                           return sb.ToString();
                       }
                   }
                   """,
        Hint: "Trace through 'aaabb': how many times should 'a' appear? What count gets appended for 'b'?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Text;

                      /// <summary>
                      /// Implements run-length encoding (RLE) for string compression.
                      /// Used in fax transmission, BMP image codecs, and protocol framing to compress repeated character sequences.
                      /// Derive the exact encoding contract from the unit tests.
                      /// </summary>
                      public class BlackBoxHardRunLengthEncoder
                      {
                          /// <summary>
                          /// Encodes the input string using run-length encoding according to the contract described by the unit tests.
                          /// </summary>
                          /// <param name="input">The string to encode.</param>
                          /// <returns>The run-length encoded representation of <paramref name="input"/>.</returns>
                          public string Encode(string input)
                          {
                              if (string.IsNullOrEmpty(input)) return "";
                              var sb = new StringBuilder();
                              int count = 1;
                              for (int i = 1; i <= input.Length; i++)
                              {
                                  if (i < input.Length && input[i] == input[i - 1])
                                  {
                                      count++;
                                  }
                                  else
                                  {
                                      sb.Append(count);
                                      sb.Append(input[i - 1]);
                                      count = 1;
                                  }
                              }
                              return sb.ToString();
                          }
                      }
                      """,
        Explanation:
        "Reading the tests reveals the encoding format: each run is represented as a count followed by the character, e.g. \"aaabb\" → \"3a2b\". " +
        "The bug resets count to 0 at the end of each run instead of 1. " +
        "Because the next character has already been scanned, the new run has actually started — the counter should begin at 1, not 0. " +
        "With count = 0, the second and subsequent distinct characters always get an undercount of 1 (they appear as 0 increments + the final append), producing incorrect output like \"3a1b\" for input 'aaabb'. " +
        "The fix is count = 1 to correctly account for the character that just triggered the flush.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class BlackBoxHardRunLengthEncoderTests
                  {
                      [Fact]
                      public void Encode_RepeatingGroups_ReturnsCorrectEncoding()
                      {
                          var sut = new BlackBoxHardRunLengthEncoder();
                          Assert.Equal("3a2b", sut.Encode("aaabb"));
                      }

                      [Fact]
                      public void Encode_AllUnique_EachCountIsOne()
                      {
                          var sut = new BlackBoxHardRunLengthEncoder();
                          Assert.Equal("1a1b1c", sut.Encode("abc"));
                      }

                      [Fact]
                      public void Encode_EmptyString_ReturnsEmpty()
                      {
                          var sut = new BlackBoxHardRunLengthEncoder();
                          Assert.Equal("", sut.Encode(""));
                      }

                      [Fact]
                      public void Encode_SingleChar_ReturnsOneAndChar()
                      {
                          var sut = new BlackBoxHardRunLengthEncoder();
                          Assert.Equal("1z", sut.Encode("z"));
                      }
                  }
                  """
    );

    private static Template LogTime(Difficulties d) => d switch
    {
        Difficulties.Easy => LogTimeEasy(),
        Difficulties.Medium => LogTimeMedium(),
        Difficulties.Hard => LogTimeHard(),
        _ => LogTimeEasy()
    };

    private static Template LogTimeEasy() => new(
        ClassName: "LogTimeEasyDuplicateChecker",
        Description:
        "Duplicate detection is required in user-registration services (detecting duplicate email addresses), inventory systems (preventing duplicate SKUs), and data-ingestion pipelines. " +
        "The current implementation has two problems: a self-comparison bug that always returns true for non-empty lists, and an O(n²) nested-loop strategy that degrades badly on large inputs. " +
        "Fix both the correctness bug AND improve the time complexity to O(n) by using a HashSet.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Detects duplicate values in integer collections.
                   /// Used in user-registration services, inventory systems, and data-ingestion pipelines
                   /// where duplicate entries must be identified efficiently.
                   /// </summary>
                   public class LogTimeEasyDuplicateChecker
                   {
                       /// <summary>
                       /// Determines whether the list contains at least one duplicate value.
                       /// Must run in O(n) time.
                       /// </summary>
                       /// <param name="numbers">The list of integers to inspect.</param>
                       /// <returns><c>true</c> if any value appears more than once; otherwise <c>false</c>.</returns>
                       // TODO: Something is wrong here – time complexity is O(n²) and there is a bug
                       public bool HasDuplicate(List<int> numbers)
                       {
                           for (int i = 0; i < numbers.Count; i++)
                               for (int j = i; j < numbers.Count; j++)
                                   if (numbers[i] == numbers[j])
                                       return true;
                           return false;
                       }
                   }
                   """,
        Hint:
        "A HashSet tracks seen elements in O(1). If HashSet.Add() returns false, the element was already present. Also: should j start at i or i+1?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Detects duplicate values in integer collections.
                      /// Used in user-registration services, inventory systems, and data-ingestion pipelines
                      /// where duplicate entries must be identified efficiently.
                      /// </summary>
                      public class LogTimeEasyDuplicateChecker
                      {
                          /// <summary>
                          /// Determines whether the list contains at least one duplicate value.
                          /// Runs in O(n) time using a HashSet.
                          /// </summary>
                          /// <param name="numbers">The list of integers to inspect.</param>
                          /// <returns><c>true</c> if any value appears more than once; otherwise <c>false</c>.</returns>
                          public bool HasDuplicate(List<int> numbers)
                          {
                              var seen = new HashSet<int>();
                              foreach (var n in numbers)
                                  if (!seen.Add(n)) return true;
                              return false;
                          }
                      }
                      """,
        Explanation:
        "Two separate problems existed in the original code. " +
        "First, the inner loop starts at j = i instead of j = i + 1, which means every element is compared with itself — numbers[i] == numbers[i] is always true, so the method always returns true for non-empty lists even when there are no duplicates. " +
        "Second, the O(n²) nested loop is too slow for large inputs. " +
        "The O(n) fix uses a HashSet: HashSet.Add() returns false when the value is already present, " +
        "allowing duplicate detection in a single linear pass with O(1) amortised lookup per element.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class LogTimeEasyDuplicateCheckerTests
                  {
                      [Fact]
                      public void HasDuplicate_NoDuplicates_ReturnsFalse()
                      {
                          var sut = new LogTimeEasyDuplicateChecker();
                          Assert.False(sut.HasDuplicate([1, 2, 3, 4]));
                      }

                      [Fact]
                      public void HasDuplicate_WithDuplicate_ReturnsTrue()
                      {
                          var sut = new LogTimeEasyDuplicateChecker();
                          Assert.True(sut.HasDuplicate([1, 2, 3, 1]));
                      }

                      [Fact]
                      public void HasDuplicate_EmptyList_ReturnsFalse()
                      {
                          var sut = new LogTimeEasyDuplicateChecker();
                          Assert.False(sut.HasDuplicate([]));
                      }
                  }
                  """
    );

    private static Template LogTimeMedium() => new(
        ClassName: "LogTimeMediumTwoSum",
        Description:
        "Two-sum lookups underpin budget-matching features in expense-tracking apps, two-factor authentication token pairing, and combinatorial search problems in algorithm-interview platforms. " +
        "This method should return the indices of the two distinct elements that sum to a target value. " +
        "The current implementation has a self-pairing bug (an element can pair with itself) and runs in O(n²). " +
        "Fix both the correctness bug AND improve the time complexity to O(n) using a Dictionary.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Solves the Two Sum problem for integer arrays.
                   /// Used in budget-matching services, token-pairing utilities, and algorithm exercises
                   /// where two distinct elements summing to a target must be located efficiently.
                   /// </summary>
                   public class LogTimeMediumTwoSum
                   {
                       /// <summary>
                       /// Returns the zero-based indices of the two distinct elements that sum to <paramref name="target"/>.
                       /// Must run in O(n) time.
                       /// </summary>
                       /// <param name="nums">The array of integers to search.</param>
                       /// <param name="target">The target sum.</param>
                       /// <returns>
                       /// An array containing the two indices [i, j] where i &lt; j and nums[i] + nums[j] == target,
                       /// or [-1, -1] if no such pair exists.
                       /// </returns>
                       // TODO: Something is wrong here – O(n²) and self-pairing bug
                       public int[] TwoSum(int[] nums, int target)
                       {
                           for (int i = 0; i < nums.Length; i++)
                               for (int j = i; j < nums.Length; j++)
                                   if (nums[i] + nums[j] == target)
                                       return [i, j];
                           return [-1, -1];
                       }
                   }
                   """,
        Hint:
        "Track previously seen numbers in a Dictionary<value, index>. For each element, check whether (target - element) is already stored.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Solves the Two Sum problem for integer arrays.
                      /// Used in budget-matching services, token-pairing utilities, and algorithm exercises
                      /// where two distinct elements summing to a target must be located efficiently.
                      /// </summary>
                      public class LogTimeMediumTwoSum
                      {
                          /// <summary>
                          /// Returns the zero-based indices of the two distinct elements that sum to <paramref name="target"/>.
                          /// Runs in O(n) time using a Dictionary.
                          /// </summary>
                          /// <param name="nums">The array of integers to search.</param>
                          /// <param name="target">The target sum.</param>
                          /// <returns>
                          /// An array containing the two indices [i, j] where i &lt; j and nums[i] + nums[j] == target,
                          /// or [-1, -1] if no such pair exists.
                          /// </returns>
                          public int[] TwoSum(int[] nums, int target)
                          {
                              var seen = new Dictionary<int, int>();
                              for (int i = 0; i < nums.Length; i++)
                              {
                                  int complement = target - nums[i];
                                  if (seen.TryGetValue(complement, out int j))
                                      return [j, i];
                                  seen[nums[i]] = i;
                              }
                              return [-1, -1];
                          }
                      }
                      """,
        Explanation:
        "Two bugs existed. First, starting the inner loop at j = i allows an element to be paired with itself (e.g. nums[0] + nums[0] == target when nums[0] == target / 2), violating the requirement for two distinct indices. " +
        "Changing to j = i + 1 fixes the self-pairing. " +
        "Second, the O(n²) nested loops are eliminated by the O(n) Dictionary approach: for each element, compute its complement (target - nums[i]) and check in O(1) whether that value was seen earlier. " +
        "If found, the stored index and the current index form the answer; otherwise store the current value and move on.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class LogTimeMediumTwoSumTests
                  {
                      [Fact]
                      public void TwoSum_DistinctPair_ReturnsCorrectIndices()
                      {
                          var sut = new LogTimeMediumTwoSum();
                          Assert.Equal([0, 1], sut.TwoSum([2, 7, 11, 15], 9));
                      }

                      [Fact]
                      public void TwoSum_SameValueTwice_ReturnsDistinctIndices()
                      {
                          var sut = new LogTimeMediumTwoSum();
                          Assert.Equal([0, 1], sut.TwoSum([3, 3], 6));
                      }

                      [Fact]
                      public void TwoSum_NoMatch_ReturnsMinusOne()
                      {
                          var sut = new LogTimeMediumTwoSum();
                          Assert.Equal([-1, -1], sut.TwoSum([1, 2, 3], 10));
                      }
                  }
                  """
    );

    private static Template LogTimeHard() => new(
        ClassName: "LogTimeHardNearbyDuplicate",
        Description:
        "Proximity-duplicate detection is used in fraud-detection systems (the same transaction ID appearing within a short window), sensor deduplication in IoT pipelines, and event-log anomaly detection. " +
        "This method should return true if any two equal elements appear within k index positions of each other in the array. " +
        "The current implementation has an inverted comparison operator (returning true when elements are far apart instead of close together) and runs in O(n²). " +
        "Fix the correctness bug AND improve the time complexity to O(n) using a Dictionary.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Detects proximity duplicates — equal values that appear within a given index distance.
                   /// Used in fraud-detection systems, IoT sensor deduplication, and event-log anomaly detection.
                   /// </summary>
                   public class LogTimeHardNearbyDuplicate
                   {
                       /// <summary>
                       /// Determines whether any two equal elements in <paramref name="nums"/> are at most
                       /// <paramref name="k"/> index positions apart.
                       /// Must run in O(n) time.
                       /// </summary>
                       /// <param name="nums">The array of integers to inspect.</param>
                       /// <param name="k">The maximum allowed index distance between equal elements.</param>
                       /// <returns>
                       /// <c>true</c> if there exist indices i and j such that nums[i] == nums[j] and |i - j| &lt;= k;
                       /// otherwise <c>false</c>.
                       /// </returns>
                       // TODO: Something is wrong here – O(n²) and wrong comparison direction
                       public bool ContainsNearbyDuplicate(int[] nums, int k)
                       {
                           for (int i = 0; i < nums.Length; i++)
                               for (int j = i + 1; j < nums.Length; j++)
                                   if (nums[i] == nums[j] && j - i > k)
                                       return true;
                           return false;
                       }
                   }
                   """,
        Hint:
        "Keep the most recent index of each value in a Dictionary. If you see it again and the distance is ≤ k, return true.",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Detects proximity duplicates — equal values that appear within a given index distance.
                      /// Used in fraud-detection systems, IoT sensor deduplication, and event-log anomaly detection.
                      /// </summary>
                      public class LogTimeHardNearbyDuplicate
                      {
                          /// <summary>
                          /// Determines whether any two equal elements in <paramref name="nums"/> are at most
                          /// <paramref name="k"/> index positions apart.
                          /// Runs in O(n) time using a Dictionary.
                          /// </summary>
                          /// <param name="nums">The array of integers to inspect.</param>
                          /// <param name="k">The maximum allowed index distance between equal elements.</param>
                          /// <returns>
                          /// <c>true</c> if there exist indices i and j such that nums[i] == nums[j] and |i - j| &lt;= k;
                          /// otherwise <c>false</c>.
                          /// </returns>
                          public bool ContainsNearbyDuplicate(int[] nums, int k)
                          {
                              var seen = new Dictionary<int, int>();
                              for (int i = 0; i < nums.Length; i++)
                              {
                                  if (seen.TryGetValue(nums[i], out int prev) && i - prev <= k)
                                      return true;
                                  seen[nums[i]] = i;
                              }
                              return false;
                          }
                      }
                      """,
        Explanation:
        "The condition j - i > k returns true when the two equal elements are more than k positions apart — the exact opposite of the requirement (≤ k means nearby). " +
        "This inversion means the method flags far-apart duplicates as valid and misses nearby ones, producing entirely wrong results. " +
        "Changing > to <= fixes the comparison. " +
        "The O(n) improvement stores each value's most-recent index in a Dictionary; on revisiting a value, the stored index is compared with the current one in O(1), " +
        "updating the stored index afterwards so only the closest previous occurrence is considered.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class LogTimeHardNearbyDuplicateTests
                  {
                      [Fact]
                      public void ContainsNearbyDuplicate_DuplicateWithinK_ReturnsTrue()
                      {
                          var sut = new LogTimeHardNearbyDuplicate();
                          Assert.True(sut.ContainsNearbyDuplicate([1, 2, 3, 1], 3));
                      }

                      [Fact]
                      public void ContainsNearbyDuplicate_DuplicateOutsideK_ReturnsFalse()
                      {
                          var sut = new LogTimeHardNearbyDuplicate();
                          Assert.False(sut.ContainsNearbyDuplicate([1, 2, 3, 1], 2));
                      }

                      [Fact]
                      public void ContainsNearbyDuplicate_NoDuplicates_ReturnsFalse()
                      {
                          var sut = new LogTimeHardNearbyDuplicate();
                          Assert.False(sut.ContainsNearbyDuplicate([1, 2, 3, 4], 2));
                      }
                  }
                  """
    );

    private static Template General(Difficulties d) => d switch
    {
        Difficulties.Easy => GeneralEasy(),
        Difficulties.Medium => GeneralMedium(),
        Difficulties.Hard => GeneralHard(),
        _ => GeneralEasy()
    };

    private static Template GeneralEasy() => new(
        ClassName: "GeneralEasyBankAccount",
        Description:
        "Bank-account domain models are a fundamental building block in fintech applications, payment gateways, and ledger services where financial integrity must be guaranteed by the domain layer. " +
        "This class represents a simple account that supports deposits and withdrawals, but the Withdraw method fails to enforce a minimum-balance constraint — it allows the balance to go negative without raising an error. " +
        "Find the missing guard and fix it so that overdrafts throw an appropriate exception.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   /// <summary>
                   /// Represents a bank account with deposit and withdrawal operations.
                   /// Used in fintech applications and ledger services to model account balances
                   /// while enforcing financial-integrity constraints such as overdraft prevention.
                   /// </summary>
                   public class GeneralEasyBankAccount
                   {
                       private decimal _balance;

                       /// <summary>
                       /// Initialises a new bank account with the specified opening balance.
                       /// </summary>
                       /// <param name="initialBalance">The opening balance. Must be non-negative.</param>
                       public GeneralEasyBankAccount(decimal initialBalance)
                       {
                           _balance = initialBalance;
                       }

                       /// <summary>
                       /// Deducts the specified amount from the account balance.
                       /// </summary>
                       /// <param name="amount">The amount to withdraw. Must not exceed the current balance.</param>
                       /// <exception cref="System.InvalidOperationException">
                       /// Thrown when <paramref name="amount"/> exceeds the current balance.
                       /// </exception>
                       // TODO: Something is wrong here
                       public void Withdraw(decimal amount)
                       {
                           _balance -= amount;
                       }

                       /// <summary>Returns the current account balance.</summary>
                       public decimal GetBalance() => _balance;
                   }
                   """,
        Hint: "What should happen if the withdrawal amount is greater than the current balance?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      /// <summary>
                      /// Represents a bank account with deposit and withdrawal operations.
                      /// Used in fintech applications and ledger services to model account balances
                      /// while enforcing financial-integrity constraints such as overdraft prevention.
                      /// </summary>
                      public class GeneralEasyBankAccount
                      {
                          private decimal _balance;

                          /// <summary>
                          /// Initialises a new bank account with the specified opening balance.
                          /// </summary>
                          /// <param name="initialBalance">The opening balance. Must be non-negative.</param>
                          public GeneralEasyBankAccount(decimal initialBalance)
                          {
                              _balance = initialBalance;
                          }

                          /// <summary>
                          /// Deducts the specified amount from the account balance.
                          /// </summary>
                          /// <param name="amount">The amount to withdraw. Must not exceed the current balance.</param>
                          /// <exception cref="System.InvalidOperationException">
                          /// Thrown when <paramref name="amount"/> exceeds the current balance.
                          /// </exception>
                          public void Withdraw(decimal amount)
                          {
                              if (amount > _balance)
                                  throw new System.InvalidOperationException("Insufficient funds.");
                              _balance -= amount;
                          }

                          /// <summary>Returns the current account balance.</summary>
                          public decimal GetBalance() => _balance;
                      }
                      """,
        Explanation:
        "Without the guard, any positive amount can be withdrawn regardless of the current balance, silently pushing it into a negative state. " +
        "In a real banking system this would constitute an unauthorised overdraft — a serious data-integrity violation. " +
        "The fix adds a pre-condition check: if amount > _balance, an InvalidOperationException is thrown with a descriptive message before any mutation occurs. " +
        "The subtraction only happens after the check passes, ensuring the invariant that _balance >= 0 is maintained at all times.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class GeneralEasyBankAccountTests
                  {
                      [Fact]
                      public void Withdraw_ValidAmount_ReducesBalance()
                      {
                          var account = new GeneralEasyBankAccount(100m);
                          account.Withdraw(40m);
                          Assert.Equal(60m, account.GetBalance());
                      }

                      [Fact]
                      public void Withdraw_InsufficientFunds_ThrowsInvalidOperation()
                      {
                          var account = new GeneralEasyBankAccount(30m);
                          Assert.Throws<System.InvalidOperationException>(() => account.Withdraw(100m));
                      }

                      [Fact]
                      public void Withdraw_ExactBalance_LeavesZero()
                      {
                          var account = new GeneralEasyBankAccount(50m);
                          account.Withdraw(50m);
                          Assert.Equal(0m, account.GetBalance());
                      }
                  }
                  """
    );

    private static Template GeneralMedium() => new(
        ClassName: "GeneralMediumShoppingCart",
        Description:
        "Shopping-cart domain models are a core component of every e-commerce back-end, responsible for maintaining line-item state and computing order totals that feed into payment processing and invoice generation. " +
        "This class stores items with a price and a quantity, but the GetTotal method ignores the quantity field and sums only the unit prices, producing incorrect totals whenever an item is added with a quantity greater than one. " +
        "Find the faulty calculation and fix it.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   /// <summary>
                   /// Manages a collection of line items for an e-commerce shopping cart.
                   /// Used to accumulate order contents and compute the total amount due for payment processing.
                   /// </summary>
                   public class GeneralMediumShoppingCart
                   {
                       private readonly List<(string Item, decimal Price, int Qty)> _items = new();

                       /// <summary>
                       /// Adds a product to the cart with the specified price and optional quantity.
                       /// </summary>
                       /// <param name="item">The product name or SKU.</param>
                       /// <param name="price">The unit price of the product.</param>
                       /// <param name="quantity">The number of units to add. Defaults to 1.</param>
                       public void Add(string item, decimal price, int quantity = 1)
                       {
                           _items.Add((item, price, quantity));
                       }

                       /// <summary>
                       /// Calculates the total cost of all items in the cart.
                       /// Each line item contributes Price × Quantity to the total.
                       /// </summary>
                       /// <returns>The sum of (Price × Quantity) for every line item in the cart.</returns>
                       // TODO: Something is wrong here
                       public decimal GetTotal()
                       {
                           decimal total = 0;
                           foreach (var item in _items)
                               total += item.Price;
                           return total;
                       }
                   }
                   """,
        Hint: "How should price and quantity be combined for each line item?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System.Collections.Generic;

                      /// <summary>
                      /// Manages a collection of line items for an e-commerce shopping cart.
                      /// Used to accumulate order contents and compute the total amount due for payment processing.
                      /// </summary>
                      public class GeneralMediumShoppingCart
                      {
                          private readonly List<(string Item, decimal Price, int Qty)> _items = new();

                          /// <summary>
                          /// Adds a product to the cart with the specified price and optional quantity.
                          /// </summary>
                          /// <param name="item">The product name or SKU.</param>
                          /// <param name="price">The unit price of the product.</param>
                          /// <param name="quantity">The number of units to add. Defaults to 1.</param>
                          public void Add(string item, decimal price, int quantity = 1)
                          {
                              _items.Add((item, price, quantity));
                          }

                          /// <summary>
                          /// Calculates the total cost of all items in the cart.
                          /// Each line item contributes Price × Quantity to the total.
                          /// </summary>
                          /// <returns>The sum of (Price × Quantity) for every line item in the cart.</returns>
                          public decimal GetTotal()
                          {
                              decimal total = 0;
                              foreach (var item in _items)
                                  total += item.Price * item.Qty;
                              return total;
                          }
                      }
                      """,
        Explanation:
        "GetTotal accumulated item.Price without multiplying by item.Qty, effectively treating every line item as quantity 1 regardless of how many units were added. " +
        "For example, adding 3 apples at £1.00 each would contribute only £1.00 to the total instead of £3.00. " +
        "This kind of bug is easy to introduce when the quantity field is added to the data model after the calculation logic is already written. " +
        "The fix is to change the accumulation to item.Price * item.Qty so each line item's contribution is correctly scaled by its quantity.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class GeneralMediumShoppingCartTests
                  {
                      [Fact]
                      public void GetTotal_QuantityGreaterThanOne_MultipliesPriceByQuantity()
                      {
                          var cart = new GeneralMediumShoppingCart();
                          cart.Add("Apple", 1.0m, 3);
                          Assert.Equal(3.0m, cart.GetTotal());
                      }

                      [Fact]
                      public void GetTotal_DefaultQuantity_ReturnsPriceOnce()
                      {
                          var cart = new GeneralMediumShoppingCart();
                          cart.Add("Book", 20m);
                          Assert.Equal(20m, cart.GetTotal());
                      }

                      [Fact]
                      public void GetTotal_MultipleItems_ReturnsCombinedTotal()
                      {
                          var cart = new GeneralMediumShoppingCart();
                          cart.Add("Pen", 2m, 5);
                          cart.Add("Notebook", 10m, 2);
                          Assert.Equal(30m, cart.GetTotal());
                      }
                  }
                  """
    );

    private static Template GeneralHard() => new(
        ClassName: "GeneralHardEventEmitter",
        Description:
        "Event emitters and publish-subscribe mechanisms are fundamental infrastructure in UI frameworks, message buses, and reactive systems — they allow components to communicate without tight coupling. " +
        "This class exposes Subscribe and Unsubscribe methods for registering and deregistering message handlers, plus an Emit method that invokes all current subscribers. " +
        "A copy-paste bug in Unsubscribe causes the handler to be added again rather than removed, so subscribers can never detach. " +
        "Find and fix the operator error.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System;

                   /// <summary>
                   /// Provides a lightweight publish-subscribe event emitter.
                   /// Used in UI frameworks, message buses, and reactive systems to decouple event producers from consumers.
                   /// </summary>
                   public class GeneralHardEventEmitter
                   {
                       /// <summary>Raised when a message is emitted. Null when there are no subscribers.</summary>
                       public event Action<string>? OnMessage;

                       /// <summary>
                       /// Broadcasts <paramref name="message"/> to all current subscribers.
                       /// </summary>
                       /// <param name="message">The message payload to deliver.</param>
                       public void Emit(string message)
                       {
                           OnMessage?.Invoke(message);
                       }

                       /// <summary>
                       /// Registers <paramref name="handler"/> to receive future messages and returns it
                       /// so the caller can store it for later unsubscription.
                       /// </summary>
                       /// <param name="handler">The callback to register.</param>
                       /// <returns>The same <paramref name="handler"/> instance, for use with <see cref="Unsubscribe"/>.</returns>
                       public Action<string> Subscribe(Action<string> handler)
                       {
                           OnMessage += handler;
                           return handler;
                       }

                       /// <summary>
                       /// Removes <paramref name="handler"/> from the subscriber list so it no longer receives messages.
                       /// </summary>
                       /// <param name="handler">The callback to deregister. Must be the same instance passed to <see cref="Subscribe"/>.</param>
                       // TODO: Something is wrong here
                       public void Unsubscribe(Action<string> handler)
                       {
                           OnMessage += handler;
                       }
                   }
                   """,
        Hint: "Check the operator used in Unsubscribe. Which operator removes a delegate from a multicast event?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;
                      using System;

                      /// <summary>
                      /// Provides a lightweight publish-subscribe event emitter.
                      /// Used in UI frameworks, message buses, and reactive systems to decouple event producers from consumers.
                      /// </summary>
                      public class GeneralHardEventEmitter
                      {
                          /// <summary>Raised when a message is emitted. Null when there are no subscribers.</summary>
                          public event Action<string>? OnMessage;

                          /// <summary>
                          /// Broadcasts <paramref name="message"/> to all current subscribers.
                          /// </summary>
                          /// <param name="message">The message payload to deliver.</param>
                          public void Emit(string message)
                          {
                              OnMessage?.Invoke(message);
                          }

                          /// <summary>
                          /// Registers <paramref name="handler"/> to receive future messages and returns it
                          /// so the caller can store it for later unsubscription.
                          /// </summary>
                          /// <param name="handler">The callback to register.</param>
                          /// <returns>The same <paramref name="handler"/> instance, for use with <see cref="Unsubscribe"/>.</returns>
                          public Action<string> Subscribe(Action<string> handler)
                          {
                              OnMessage += handler;
                              return handler;
                          }

                          /// <summary>
                          /// Removes <paramref name="handler"/> from the subscriber list so it no longer receives messages.
                          /// </summary>
                          /// <param name="handler">The callback to deregister. Must be the same instance passed to <see cref="Subscribe"/>.</param>
                          public void Unsubscribe(Action<string> handler)
                          {
                              OnMessage -= handler;
                          }
                      }
                      """,
        Explanation:
        "Unsubscribe used the += operator instead of -=, which adds the handler to the multicast delegate a second time rather than removing it. " +
        "After calling Unsubscribe, the handler was therefore still (and actually doubly) subscribed, so it continued to receive all subsequent messages — a classic copy-paste error. " +
        "In C#, multicast delegates are combined with += and a specific invocation is removed with -=. " +
        "The fix is the single-character change from += to -=, which causes the runtime to create a new delegate chain that excludes the specified handler instance.",
        TestCode: """
                  namespace Buggernaut.Tests;
                  using Xunit;
                  using Buggernaut.Exercises;

                  public class GeneralHardEventEmitterTests
                  {
                      [Fact]
                      public void Unsubscribe_HandlerNoLongerReceivesMessages()
                      {
                          var emitter = new GeneralHardEventEmitter();
                          int callCount = 0;
                          var handler = emitter.Subscribe(msg => callCount++);
                          emitter.Emit("first");
                          emitter.Unsubscribe(handler);
                          emitter.Emit("second");
                          Assert.Equal(1, callCount);
                      }

                      [Fact]
                      public void Subscribe_HandlerReceivesEmittedMessages()
                      {
                          var emitter = new GeneralHardEventEmitter();
                          string received = "";
                          emitter.Subscribe(msg => received = msg);
                          emitter.Emit("hello");
                          Assert.Equal("hello", received);
                      }

                      [Fact]
                      public void Emit_NoSubscribers_DoesNotThrow()
                      {
                          var emitter = new GeneralHardEventEmitter();
                          var ex = Record.Exception(() => emitter.Emit("test"));
                          Assert.Null(ex);
                      }
                  }
                  """
    );
}