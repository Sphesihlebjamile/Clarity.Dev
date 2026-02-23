namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Parses individual project files and extracts metadata 🦢
/// </summary>
public interface IProjectParser
{
    /// <summary>
    /// Parses a Roslyn project and extracts all relevant information 🦢
    /// </summary>
    /// <param name="project"></param>
    /// <param name="manager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<SolutionModels.ProjectInfo> ParseProjectAsync(
        Project project,
        AnalyzerManager? manager,
        CancellationToken cancellationToken = default);
}
