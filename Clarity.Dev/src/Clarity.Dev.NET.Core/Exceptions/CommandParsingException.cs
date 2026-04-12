namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Thrown when one or more command-line arguments cannot be parsed.
/// Exit code: 2
/// </summary>
public class CommandParsingException : CliException
{
    public override int ExitCode => 2;

    public CommandParsingException(string message) : base(message) { }

    public CommandParsingException(string message, Exception innerException) : base(message, innerException) { }
}
