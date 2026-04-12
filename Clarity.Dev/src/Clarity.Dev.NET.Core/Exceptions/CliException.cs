namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Base exception for all Clarity.Dev CLI domain errors.
/// </summary>
public abstract class CliException : Exception
{
    /// <summary>
    /// The exit code this exception maps to when caught at the application boundary.
    /// </summary>
    public abstract int ExitCode { get; }

    protected CliException(string message) : base(message) { }

    protected CliException(string message, Exception innerException) : base(message, innerException) { }
}
