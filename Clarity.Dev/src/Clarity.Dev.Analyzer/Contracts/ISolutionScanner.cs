namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Main orchestrator for analyzing .NET solutions 🦢
/// </summary>
public interface ISolutionScanner
{
    /// <summary>
    /// Analyzes a .NET solution (.sln or .slnx) and returns comprehensive analysis results 🦢
    /// </summary>
    /// <param name="solutionPath">relative/absolute path to the .sln/.slnx file</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<SolutionModels.SolutionAnalysisResult> AnalyzeSolutionAsync(
        string solutionPath,
        CancellationToken cancellationToken = default);
}
