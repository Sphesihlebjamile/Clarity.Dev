namespace Clarity.Dev.NET.Core.Models.Contracts;

public interface IAnalysisCommand
{
    string SolutionPath { get; set; }
    string OutputFormat { get; set; }
    string OutputPath { get; set; }
    bool IsHelp { get; set; }
    bool IsVersion { get; set; }
}
