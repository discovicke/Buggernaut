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
        "The method should count the number of vowels in a string but returns the wrong number. Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class BugEasyVowelCounter
                   {
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

                      public class BugEasyVowelCounter
                      {
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
        Explanation: "The vowel string \"aeio\" is missing 'u'. The correct string is \"aeiou\".",
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
        "The method should count how many times each word appears in a sentence. The result is wrong — find the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class BugMediumWordCounter
                   {
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

                      public class BugMediumWordCounter
                      {
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
        "New words were initialized to 0 instead of 1. Each word seen for the first time should have a count of 1.",
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
        "The method should verify that a string has correctly matched brackets. It returns the wrong answer for some inputs — find the subtle bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class BugHardBracketValidator
                   {
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

                      public class BugHardBracketValidator
                      {
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
        "The condition for ']' checked against '{' (curly brace) instead of '[' (square bracket). ']' must be matched against '[' on the stack.",
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
        "What does FizzBuzz(15) return? The code looks almost correct — but is it really? Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class GuessOutputEasyFizzBuzz
                   {
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

                      public class GuessOutputEasyFizzBuzz
                      {
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
        "The % 15 condition must be checked BEFORE % 3 and % 5, otherwise 15 matches the wrong condition first.",
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
        "The method builds a comma-separated string from a list. What does it return for [\"a\",\"b\",\"c\"]? Is there a bug?",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Text;

                   public class GuessOutputMediumStringBuilder
                   {
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

                      public class GuessOutputMediumStringBuilder
                      {
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
        "The original code appends a trailing \", \" after the last element. The fix only appends a comma between elements.",
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
        "The method returns a list of Func delegates. What do they return? There is a classic C# closure trap here.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System;
                   using System.Collections.Generic;

                   public class GuessOutputHardClosureCapture
                   {
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

                      public class GuessOutputHardClosureCapture
                      {
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
        "The lambda captures a reference to i, not its value. When the loop ends i == count, so all delegates return count. Fix: copy i to a local variable inside the loop.",
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
        Description: "The method should return the largest number in a list. The implementation is missing — fill it in.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class FillInTheGapEasyMaxFinder
                   {
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

                      public class FillInTheGapEasyMaxFinder
                      {
                          public int FindMax(List<int> numbers)
                          {
                              int max = numbers[0];
                              foreach (var n in numbers)
                                  if (n > max) max = n;
                              return max;
                          }
                      }
                      """,
        Explanation: "Initialise max with the first element and update it whenever a larger number is found.",
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
        Description: "The method should determine whether two strings are anagrams of each other. The implementation is missing — fill it in.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class FillInTheGapMediumAnagram
                   {
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

                      public class FillInTheGapMediumAnagram
                      {
                          public bool IsAnagram(string a, string b)
                          {
                              if (a.Length != b.Length) return false;
                              var sortedA = string.Concat(a.ToLower().OrderBy(c => c));
                              var sortedB = string.Concat(b.ToLower().OrderBy(c => c));
                              return sortedA == sortedB;
                          }
                      }
                      """,
        Explanation: "Sort the characters in both strings and compare. If they are identical, the strings are anagrams.",
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
        "Implement an LRU cache (Least Recently Used) with a fixed capacity. Get should return -1 if the key does not exist. Put should evict the least recently used entry when the cache is full.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class FillInTheGapHardLruCache
                   {
                       private readonly int _capacity;
                       private readonly Dictionary<int, LinkedListNode<(int key, int value)>> _map;
                       private readonly LinkedList<(int key, int value)> _list;

                       public FillInTheGapHardLruCache(int capacity)
                       {
                           _capacity = capacity;
                           _map = new Dictionary<int, LinkedListNode<(int key, int value)>>();
                           _list = new LinkedList<(int key, int value)>();
                       }

                        // TODO: Something is wrong here – implement Get and Put
                       public int Get(int key)
                       {
                           throw new System.NotImplementedException();
                       }

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

                      public class FillInTheGapHardLruCache
                      {
                          private readonly int _capacity;
                          private readonly Dictionary<int, LinkedListNode<(int key, int value)>> _map;
                          private readonly LinkedList<(int key, int value)> _list;

                          public FillInTheGapHardLruCache(int capacity)
                          {
                              _capacity = capacity;
                              _map = new Dictionary<int, LinkedListNode<(int key, int value)>>();
                              _list = new LinkedList<(int key, int value)>();
                          }

                          public int Get(int key)
                          {
                              if (!_map.TryGetValue(key, out var node)) return -1;
                              _list.Remove(node);
                              _list.AddFirst(node);
                              return node.Value.value;
                          }

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
        "LinkedList maintains order (MRU at front, LRU at back). Dictionary provides O(1) lookups. Get updates the order by moving the node to the front.",
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
        Description: "Implement a method that determines whether a given integer is a prime number.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class AlgorithmRiddleEasyIsPrime
                   {
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

                      public class AlgorithmRiddleEasyIsPrime
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
        Explanation: "Loop divisors from 2 up to √n. If n is divisible by any of them, it is not prime.",
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
        "The method recursively sums the digits of a number, but returns wrong results for multi-digit numbers. Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class AlgorithmRiddleMediumDigitSum
                   {
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

                      public class AlgorithmRiddleMediumDigitSum
                      {
                          public int SumDigits(int n)
                          {
                              n = System.Math.Abs(n);
                              if (n < 10) return n;
                              return (n % 10) + SumDigits(n / 10);
                          }
                      }
                      """,
        Explanation:
        "The recursive call used n % 10 (the last digit again) instead of n / 10 (the number without the last digit), causing the same digit to be summed repeatedly.",
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
        "The method finds the maximum subarray sum using Kadane's algorithm, but it tracks the wrong value for the global maximum. Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class AlgorithmRiddleHardMaxSubarray
                   {
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

                      public class AlgorithmRiddleHardMaxSubarray
                      {
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
        "maxSoFar was updated with nums[i] (just the current single element) instead of maxEndingHere (the best subarray ending at position i). The global max must track the best running sum.",
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
        "The method should return only the even numbers from a list, but filters the wrong subset. Fix the LINQ query.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Linq;

                   public class LinqEasyEvenFilter
                   {
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

                      public class LinqEasyEvenFilter
                      {
                          public List<int> GetEvens(List<int> numbers)
                          {
                              return numbers.Where(x => x % 2 == 0).ToList();
                          }
                      }
                      """,
        Explanation:
        "The predicate x % 2 != 0 keeps odd numbers instead of even ones. The correct condition is x % 2 == 0.",
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
        "The method should return the names of the top 3 highest-scoring players, but returns the wrong players. Fix the LINQ query.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Linq;

                   public class LinqMediumTopScorers
                   {
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

                      public class LinqMediumTopScorers
                      {
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
        "OrderBy sorts ascending (lowest first), so Take(3) picks the worst scorers. OrderByDescending is needed to get the top 3.",
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
        "The method should calculate the average price of products grouped by category, but it groups by the wrong field. Fix the LINQ query.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;
                   using System.Linq;

                   public class LinqHardGroupAverage
                   {
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

                      public class LinqHardGroupAverage
                      {
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
        "GroupBy(p => p.Name) groups by product name (one entry per product) instead of by category. The correct key is p.Category.",
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
        "Study the tests to understand what Transform should do, then find and fix the bug in the implementation.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class BlackBoxEasyTransformer
                   {
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

                      public class BlackBoxEasyTransformer
                      {
                          public int Transform(int n)
                          {
                              if (n < 0) return -n;
                              return n;
                          }
                      }
                      """,
        Explanation:
        "The method is Math.Abs — it should return the absolute value. The bug is that positive inputs are negated. The positive branch must return n, not -n.",
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
        "Study the tests to understand what Shift should do, then find and fix the bug in the implementation.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class BlackBoxMediumListShifter
                   {
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

                      public class BlackBoxMediumListShifter
                      {
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
        "The method rotates a list right by k positions — the last k elements move to the front. The bug takes elements from the front (left rotation) instead of the end (right rotation).",
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
        "Study the tests to understand what Encode should produce, then find and fix the bug in the implementation.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Text;

                   public class BlackBoxHardRunLengthEncoder
                   {
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

                      public class BlackBoxHardRunLengthEncoder
                      {
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
        "The method performs run-length encoding (e.g. \"aaabb\" → \"3a2b\"). The bug resets count to 0 instead of 1, so the next run starts undercounted — the second distinct character gets count 1 in the buggy version but loses the first occurrence.",
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
        "The method checks for duplicates in a list but has both a bug and an O(n²) time complexity. Fix the bug AND improve it to O(n) using a HashSet.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class LogTimeEasyDuplicateChecker
                   {
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

                      public class LogTimeEasyDuplicateChecker
                      {
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
        "Bug: j = i causes every element to match itself (nums[i] == nums[i] is always true). O(n²) fix: use a HashSet — Add() returns false if the element already exists, giving O(n) overall.",
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
        "The method finds two indices whose values sum to the target. It runs in O(n²) and has a self-pairing bug. Fix the bug AND improve it to O(n) using a Dictionary.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class LogTimeMediumTwoSum
                   {
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

                      public class LogTimeMediumTwoSum
                      {
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
        "j = i allows an element to pair with itself (e.g. 3+3=6 at index 0). Fix: j = i+1. O(n) solution: store each number's index in a dictionary and check if the complement was seen.",
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
        "The method checks whether any two equal elements appear within k positions of each other. It runs in O(n²) and has a reversed comparison. Fix the bug AND improve it to O(n) using a Dictionary.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class LogTimeHardNearbyDuplicate
                   {
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

                      public class LogTimeHardNearbyDuplicate
                      {
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
        "j - i > k returns true when elements are FAR apart — the opposite of the requirement. The fix is j - i <= k. The O(n) solution tracks each value's last-seen index and checks the distance on revisit.",
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
        "A bank account class allows withdrawals without checking whether sufficient funds exist. Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;

                   public class GeneralEasyBankAccount
                   {
                       private decimal _balance;

                       public GeneralEasyBankAccount(decimal initialBalance)
                       {
                           _balance = initialBalance;
                       }

                       // TODO: Something is wrong here
                       public void Withdraw(decimal amount)
                       {
                           _balance -= amount;
                       }

                       public decimal GetBalance() => _balance;
                   }
                   """,
        Hint: "What should happen if the withdrawal amount is greater than the current balance?",
        SolutionCode: """
                      namespace Buggernaut.Exercises;

                      public class GeneralEasyBankAccount
                      {
                          private decimal _balance;

                          public GeneralEasyBankAccount(decimal initialBalance)
                          {
                              _balance = initialBalance;
                          }

                          public void Withdraw(decimal amount)
                          {
                              if (amount > _balance)
                                  throw new System.InvalidOperationException("Insufficient funds.");
                              _balance -= amount;
                          }

                          public decimal GetBalance() => _balance;
                      }
                      """,
        Explanation:
        "Withdrawing without validating the balance allows it to go negative. A guard that throws InvalidOperationException when amount > _balance prevents overdrafts.",
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
        "The shopping cart calculates the total price incorrectly when items are added with a quantity greater than one. Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System.Collections.Generic;

                   public class GeneralMediumShoppingCart
                   {
                       private readonly List<(string Item, decimal Price, int Qty)> _items = new();

                       public void Add(string item, decimal price, int quantity = 1)
                       {
                           _items.Add((item, price, quantity));
                       }

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

                      public class GeneralMediumShoppingCart
                      {
                          private readonly List<(string Item, decimal Price, int Qty)> _items = new();

                          public void Add(string item, decimal price, int quantity = 1)
                          {
                              _items.Add((item, price, quantity));
                          }

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
        "The total only added Price without multiplying by Qty. The correct expression is item.Price * item.Qty to account for the quantity of each line item.",
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
        "The event emitter has a bug — after calling Unsubscribe, the handler still receives messages. Find and fix the bug.",
        BuggyCode: """
                   namespace Buggernaut.Exercises;
                   using System;

                   public class GeneralHardEventEmitter
                   {
                       public event Action<string>? OnMessage;

                       public void Emit(string message)
                       {
                           OnMessage?.Invoke(message);
                       }

                       public Action<string> Subscribe(Action<string> handler)
                       {
                           OnMessage += handler;
                           return handler;
                       }

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

                      public class GeneralHardEventEmitter
                      {
                          public event Action<string>? OnMessage;

                          public void Emit(string message)
                          {
                              OnMessage?.Invoke(message);
                          }

                          public Action<string> Subscribe(Action<string> handler)
                          {
                              OnMessage += handler;
                              return handler;
                          }

                          public void Unsubscribe(Action<string> handler)
                          {
                              OnMessage -= handler;
                          }
                      }
                      """,
        Explanation:
        "Unsubscribe used += (add) instead of -= (remove). This means the handler is added again instead of removed, causing it to receive further messages and creating a memory/correctness leak.",
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