namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Thrown when a .sln or .slnx solution file cannot be found or is of an unsupported type.
/// Exit code: 3
/// </summary>
public class SourceNotFoundException : CliException
{
    public override int ExitCode => 3;

    public string SourcePath { get; }

    public SourceNotFoundException(string sourcePath)
        : base($"Solution file not found or unsupported: '{sourcePath}'.")
    {
        SourcePath = sourcePath;
    }

    public SourceNotFoundException(string sourcePath, string reason)
        : base($"Solution file error at '{sourcePath}': {reason}")
    {
        SourcePath = sourcePath;
    }
}
