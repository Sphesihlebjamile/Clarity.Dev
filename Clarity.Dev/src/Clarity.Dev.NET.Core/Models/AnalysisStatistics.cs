namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Summary statistics about the solution 🦢
/// </summary>
public class AnalysisStatistics
{
    public int TotalProjects { get; set; }
    public int TotalNuGetPackages { get; set; }
    public int TotalProjectReferences { get; set; }
    public int TotalServices { get; set; }
    public int CircularDependencyCount { get; set; }
    public TimeSpan AnalysisDuration { get; set; }
}
