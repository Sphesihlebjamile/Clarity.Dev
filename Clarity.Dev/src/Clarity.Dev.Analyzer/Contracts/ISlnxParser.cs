namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Parses .slnc (XML-based solution) files 🦢
/// </summary>
public interface ISlnxParser
{
    /// <summary>
    /// Parses the specified SLNX file and extracts project information.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if the provided path is invalid or if the file cannot be
    /// read.
    /// </remarks>
    /// <param name="slnxPath">
    /// The full path to the SLNX file to parse. This parameter must specify an existing file; otherwise, an exception
    /// is thrown.
    /// </param>
    /// <returns>A list of strings containing the extracted project information from the SLNX file.</returns>
    public List<string> ParseSlnx(string slnxPath);
}
