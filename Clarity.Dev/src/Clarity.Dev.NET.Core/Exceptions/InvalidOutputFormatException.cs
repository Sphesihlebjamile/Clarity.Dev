namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Thrown when an invalid output format is specified via --output.
/// Exit code: 2
/// </summary>
public class InvalidOutputFormatException : CommandParsingException
{
    public string ProvidedFormat { get; }

    public InvalidOutputFormatException(string providedFormat)
        : base($"Invalid output format: '{providedFormat}'. Use a supported format such as 'html'.")
    {
        ProvidedFormat = providedFormat;
    }
}
