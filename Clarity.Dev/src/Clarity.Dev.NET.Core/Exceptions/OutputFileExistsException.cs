namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Thrown when the specified output file already exists.
/// Exit code: 2
/// </summary>
public class OutputFileExistsException : CommandParsingException
{
    public string FilePath { get; }

    public OutputFileExistsException(string filePath)
        : base($"Output file already exists: '{filePath}'. Specify a different output path.")
    {
        FilePath = filePath;
    }
}
