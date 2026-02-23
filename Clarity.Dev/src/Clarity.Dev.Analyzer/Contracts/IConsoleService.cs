namespace Clarity.Dev.NET.Analyzer.Contracts;

public interface IConsoleService
{
    public void DisplayHeader(string version);
    public void DisplayLineSeparator();
    public void DisplayNewLine();
    public void DisplayInfo(string message);
    public void DisplaySuccess(string message);
    public void DisplayWarning(string message);
    public void DisplayError(string message);
    public void DisplayErrorWithStackTrace(string message, string stackTrace);
    public void SetForegroundColor(ConsoleColor color);
    public void ResetColor();
}