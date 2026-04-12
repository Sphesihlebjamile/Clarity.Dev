namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Parses individual project files and extracts metadata 🦢
/// </summary>
public interface IProjectParser
{
    /// <summary>
    /// Parses the specified project and returns its information asynchronously 🦢
    /// </summary>
    /// <remarks>
    /// This method may throw exceptions if the project is invalid or if parsing fails due to other
    /// issues.
    /// </remarks>
    /// <param name="project">The project to be parsed, which contains the source code and configuration details.</param>
    /// <param name="manager">An optional analyzer manager that can be used to apply additional analysis during parsing.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation, containing the parsed project information.</returns>
    public Task<SolutionModels.ProjectInfo> ParseProjectAsync(
        Project project,
        AnalyzerManager? manager,
        CancellationToken cancellationToken = default);
}
