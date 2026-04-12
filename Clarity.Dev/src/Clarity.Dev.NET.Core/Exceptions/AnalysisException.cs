namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Thrown when the solution analysis pipeline encounters a failure.
/// Exit code: 4
/// </summary>
public class AnalysisException : CliException
{
    public override int ExitCode => 4;

    public AnalysisException(string message) : base(message) { }

    public AnalysisException(string message, Exception innerException) : base(message, innerException) { }
}
