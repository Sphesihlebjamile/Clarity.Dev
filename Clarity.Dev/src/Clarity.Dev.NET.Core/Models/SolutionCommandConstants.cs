namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Constants for solution command options 🦢
/// </summary>
public static class SolutionCommandConstants
{
    /// <summary>
    /// Specifies the path to the .sln or .slnx solution file
    /// </summary>
    public const string PathOption = "--path";

    /// <summary>
    /// Specifies the output format (e.g., HTML, JSON, BOTH)
    /// </summary>
    public const string OutputOption = "--output";

    /// <summary>
    /// Specifies the output directory path for saving analysis results
    /// </summary>
    public const string OutputPathOption = "--output-path";

    /// <summary>
    /// Represents the command-line option to display help information.
    /// </summary>
    public const string HelpOption = "-help";

    /// <summary>
    /// Represents the command-line option to display version information.
    /// </summary>
    public const string VersionOption = "--version";
}
