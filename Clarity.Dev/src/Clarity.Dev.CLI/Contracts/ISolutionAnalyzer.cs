using Clarity.Dev.NET.Core.Models.Contracts;
using Clarity.Dev.Reports.Contracts;

namespace Clarity.Dev.CLI.Contracts;

public interface ISolutionAnalyzer
{
    public Task<int> AnalyzeSolution(
        IAnalysisCommand command,
        IConsoleService consoleService,
        ISolutionScanner solutionScanner,
        IHtmlReportGenerator htmlReportGenerator);
}
