namespace Clarity.Dev.Reports.Contracts;

/// <summary>
/// Base interface for all report generators 🦢
/// </summary>
public interface IReportGenerator
{
    /// <summary>
    /// Generates a report from solution analysis results and saves it to the specified output path 🦢
    /// </summary>
    /// <param name="result">The solution analysis results to include in the report</param>
    /// <param name="outputPath">The file path where the report should be saved</param>
    /// <returns>The full path to the generated report file</returns>
    public string GenerateReport(SolutionModels.SolutionAnalysisResult result, string outputPath);
}
