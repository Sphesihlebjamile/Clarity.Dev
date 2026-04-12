namespace Clarity.Dev.CLI.Contracts;

public interface ISolutionAnalyzer
{
    public Task<int> AnalyzeSolution(
        IAnalysisCommand command,
        IConsoleService consoleService,
        ISolutionScanner solutionScanner,
        IHtmlReportGenerator htmlReportGenerator,
        CancellationToken cancellationToken = default);
}
