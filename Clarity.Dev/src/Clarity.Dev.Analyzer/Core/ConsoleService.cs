namespace Clarity.Dev.NET.Analyzer.Core;

public class ConsoleService : IConsoleService
{
    public void DisplayError(string message, string exceptionMessage = "")
    {
        this.SetForegroundColor(ConsoleColor.Red);
        this.DisplayInfo($"   Error: {message}");
        
        if (!string.IsNullOrEmpty(exceptionMessage))
        {
            this.DisplayInfo($"   Error: {exceptionMessage}");
        }

        this.ResetColor();
        this.DisplayNewLine();
    }

    public void DisplayErrorWithStackTrace(string message, string stackTrace)
    {
        if (!string.IsNullOrEmpty(stackTrace))
        {
            this.DisplayError(message);
        }
        this.SetForegroundColor(ConsoleColor.Red);
        this.DisplayInfo($"❌ Error: {message}");
        this.DisplayInfo($"   Stack Trace: {stackTrace}");
        this.ResetColor();
        this.DisplayNewLine();
    }

    public void DisplayHeader(string version)
    {
        this.ResetColor();
        this.DisplayLineSeparator();
        this.DisplayInfo("|   Clarity.Dev: Solution Analyzer    |");
        this.DisplayLineSeparator();
        this.DisplayInfo($"Version: {version}");
        this.DisplayNewLine();
    }

    public void DisplayInfo(string message)
    {
        Console.WriteLine($"{message}");
    }

    public void DisplayLineSeparator()
    {
        Console.WriteLine("=======================================");
    }

    public void DisplayNewLine()
    {
        Console.WriteLine();
    }

    public void DisplaySuccess(string message)
    {
        this.SetForegroundColor(ConsoleColor.Green);
        this.DisplayInfo($"   {message}");
        this.ResetColor();
        this.DisplayNewLine();
    }

    public void DisplayWarning(string message)
    {
        this.SetForegroundColor(ConsoleColor.Yellow);
        this.DisplayInfo($"⚠️ {message}");
        this.ResetColor();
        this.DisplayNewLine();
    }

    public void ResetColor()
    {
        Console.ResetColor();
    }

    public void SetForegroundColor(ConsoleColor color)
    {
        Console.ForegroundColor = color;
    }
}
