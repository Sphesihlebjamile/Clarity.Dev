namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Represents the complete analysis result of a .NET solution (sln, slnx) 🦢
/// </summary>
public class SolutionAnalysisResult
{
    public string SolutionName { get; set; } = string.Empty;
    public string SolutionPath { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
    public List<ProjectInfo> Projects { get; set; } = [];
    public List<CircularDependency> CircularDependencies { get; set; } = [];
    public List<CommunicationPath> ServiceCommunications { get; set; } = [];
    public AnalysisStatistics Statistics { get; set; }
}
