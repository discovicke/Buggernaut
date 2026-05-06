namespace Buggernaut.Generator;

/// <summary>
/// Central class for all terminal output. Enforces consistent hierarchy,
/// color coding and whitespace throughout the application.
///
/// Visual language:
///   [H1]  ══ RUBRIK ══════════  bold section header, full-width rule
///   [H2]  -- Underrubrik --     lighter sub-section divider
///   [OK]  ✓ Meddelande          green  – success / completed
///   [INF]   Meddelande          cyan   – neutral information
///   [WARN]  Meddelande          yellow – retryable / expected problem
///   [ERR] ✗ Meddelande          red    – fatal / unexpected error
///   [DIM]   Meddelande          dark gray – secondary / path output
/// </summary>
public static class Printer
{
    private const string CheckMark = "✓";
    private const string CrossMark = "✗";
    private const int    RuleWidth = 52;


    /// <summary>Top-level header with full-width rule. Adds blank line before.</summary>
    public static void H1(string title)
    {
        Console.WriteLine();
        var inner = $"  {title.ToUpper()}  ";
        var padding = Math.Max(0, RuleWidth - inner.Length);
        var rule = new string('=', padding / 2);
        var line = $"{rule}{inner}{new string('=', padding - padding / 2)}";

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(line);
        Console.ResetColor();
    }

    /// <summary>Sub-section header with lighter dash rule.</summary>
    public static void H2(string title)
    {
        Console.WriteLine();
        var inner = $" {title} ";
        var padding = Math.Max(0, RuleWidth - inner.Length);
        var rule = new string('-', padding / 2);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{rule}{inner}{new string('-', padding - padding / 2)}");
        Console.ResetColor();
    }

    /// <summary>Green success line. ✓ prefix.</summary>
    public static void Ok(string message, int indent = 0)
    {
        Write(ConsoleColor.Green, $"{CheckMark} {message}", indent);
    }

    /// <summary>Cyan info line. No symbol prefix.</summary>
    public static void Info(string message, int indent = 0)
    {
        Write(ConsoleColor.Cyan, message, indent);
    }

    /// <summary>Yellow warning line. No symbol prefix.</summary>
    public static void Warn(string message, int indent = 0)
    {
        Write(ConsoleColor.Yellow, message, indent);
    }

    /// <summary>Red error line. ✗ prefix.</summary>
    public static void Error(string message, int indent = 0)
    {
        Write(ConsoleColor.Red, $"{CrossMark} {message}", indent);
    }

    /// <summary>Dark gray secondary / path info line.</summary>
    public static void Dim(string message, int indent = 0)
    {
        Write(ConsoleColor.DarkGray, message, indent);
    }

    /// <summary>Plain white line – neutral body text.</summary>
    public static void Line(string message, int indent = 0)
    {
        Console.WriteLine(Indented(message, indent));
    }

    /// <summary>Empty line for visual breathing room.</summary>
    public static void Blank() => Console.WriteLine();

    /// <summary>
    /// In-place countdown. Overwrites the current line each second.
    /// Call <see cref="ClearLine"/> after the loop finishes.
    /// </summary>
    public static void Countdown(int remaining)
    {
        Console.Write($"\r{new string(' ', 4)}Återförsöker om {remaining,3} s...  ");
    }

    /// <summary>Clears the current line (used after countdown).</summary>
    public static void ClearLine()
    {
        Console.Write($"\r{new string(' ', RuleWidth)}\r");
    }

    /// <summary>Labeled key/value row – used in scaffold summaries.</summary>
    public static void KeyValue(string key, string value, int indent = 1)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(Indented($"{key,-14}", indent));
        Console.ResetColor();
        Console.WriteLine(value);
    }

    /// <summary>
    /// CLI flag documentation row with two fixed columns.
    ///   Col 1 (34 chars): flag names + argument hint  e.g. "--category, -c &lt;category&gt;"
    ///   Col 2           : description text
    /// </summary>
    public static void Flag(string flagsWithArg, string description, int indent = 1)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(Indented($"{flagsWithArg,-34}", indent));
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(description);
        Console.ResetColor();
    }
    
    private static void Write(ConsoleColor color, string message, int indent)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(Indented(message, indent));
        Console.ResetColor();
    }

    private static string Indented(string message, int level) =>
        level == 0 
            ? message 
            : $"{new string(' ', level * 4)}{message}";
}

