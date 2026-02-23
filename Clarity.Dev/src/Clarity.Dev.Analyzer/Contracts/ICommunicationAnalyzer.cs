namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Analyzes inter-service communication patterns 🦢
/// </summary>
public interface ICommunicationAnalyzer
{
    /// <summary>
    /// Analyzes the specified solution to identify communication paths such as HTTP, gRPC, message queue, and database
    /// interactions across its projects 🦢
    /// </summary>
    /// <remarks>
    /// The analysis inspects each project's documents to detect various forms of communication,
    /// including HTTP client usage, gRPC clients, message queues, and database access. The operation can be cancelled
    /// by providing a cancellation token.
    /// </remarks>
    /// <param name="solution">The solution to analyze for inter-project and external communication patterns.</param>
    /// <param name="projects">A list of project information objects representing the projects to consider during analysis.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the analysis operation.</param>
    /// <returns>
    /// A list of communication paths detected within the solution. The list is empty if no communication paths are
    /// found.
    /// </returns>
    public Task<List<CommunicationPath>> AnalyzeCommunicationAsync(
        Solution solution,
        List<SolutionModels.ProjectInfo> projects,
        CancellationToken cancellationToken = default);
}
