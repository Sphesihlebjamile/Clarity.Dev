namespace Clarity.Dev.NET.Core.Models;

public class AnalysisCommand : IAnalysisCommand
{
    public string SolutionPath { get; set; } = string.Empty;
    public string OutputFormat { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public bool IsHelp { get; set; }
    public bool IsVersion { get; set; }
}
