namespace Buggernaut.Generator;

/// <summary>
/// Animated loading spinner for long-running operations.
/// Use in a using-statement for automatic cleanup.
///
/// Example:
///   using (var spinner = new Spinner("Väntar på API..."))
///   {
///       await SomeAsyncOperation();
///   }
/// </summary>
public class Spinner : IDisposable
{
    private readonly string _message;
    private readonly Thread _thread;
    private bool _isSpinning;

    public Spinner(string message)
    {
        _message = message;
        _isSpinning = true;
        _thread = new Thread(Spin);
        _thread.IsBackground = true;
        _thread.Start();
    }

    private void Spin()
    {
        var chars = new[] { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
        int i = 0;

        while (_isSpinning)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\r[{chars[i]}] {_message}");
            Console.ResetColor();
            i = (i + 1) % chars.Length;
            Thread.Sleep(100);
        }
    }

    public void Dispose()
    {
        Stop();
    }

    /// <summary>Stop the spinner and clean the line. Idempotent.</summary>
    public void Stop()
    {
        if (!_isSpinning)
            return;

        _isSpinning = false;
        _thread.Join(TimeSpan.FromSeconds(1));

        try
        {
            Console.Write($"\r{new string(' ', Console.WindowWidth - 1)}\r");
        }
        catch
        { }
    }
}

