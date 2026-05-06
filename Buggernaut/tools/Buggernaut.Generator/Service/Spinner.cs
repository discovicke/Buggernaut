namespace Buggernaut.Generator;

/// <summary>
/// Animated loading spinner for long-running operations.
/// Can optionally render a static tail text on the same line.
/// </summary>
public class Spinner : IDisposable
{
    private readonly string _message;
    private readonly string _tailText;
    private readonly Thread _thread;
    private bool _isSpinning;

    public Spinner(string message, string tailText = "")
    {
        _message = message;
        _tailText = tailText;
        _isSpinning = true;
        _thread = new Thread(Spin) { IsBackground = true };
        _thread.Start();
    }

    private void Spin()
    {
        var chars = new[] { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
        var i = 0;

        while (_isSpinning)
        {
            Console.Write("\r");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{chars[i]}] {_message}");

            if (!string.IsNullOrWhiteSpace(_tailText))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"  {_tailText}");
            }

            Console.ResetColor();
            i = (i + 1) % chars.Length;
            Thread.Sleep(100);
        }
    }

    public void Dispose()
    {
        Stop(success: false);
    }

    /// <summary>
    /// Stop spinner and render final status marker.
    /// success=true  => green check
    /// success=false => red X
    /// </summary>
    public void Stop(bool success)
    {
        if (!_isSpinning)
            return;

        _isSpinning = false;
        _thread.Join(TimeSpan.FromSeconds(1));

        Console.Write("\r");

        Console.ForegroundColor = success 
            ? ConsoleColor.Green 
            : ConsoleColor.Red;
        Console.Write(success 
            ? $"[✓] {_message}" 
            : $"[X] {_message}");

        if (!string.IsNullOrWhiteSpace(_tailText))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  {_tailText}");
        }

        Console.ResetColor();
    }
}
