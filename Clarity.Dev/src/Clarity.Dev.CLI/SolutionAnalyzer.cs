using Clarity.Dev.NET.Core.Models.Contracts;

namespace Clarity.Dev.CLI;

internal static class SolutionAnalyzer
{
    public static async Task<int> AnalyzeSolution(IAnalysisCommand command, IConsoleService consoleService)
    {
        try
        {

            if (!File.Exists(command.SolutionPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                consoleService.DisplayInfo($"❌ Error: Solution file not found: {command.SolutionPath}");
                Console.ResetColor();
                return 1;
            }

            ProjectParser _projectParser = new();
            ServiceDetector _serviceDetector = new();
            CommunicationAnalyzer _communicationAnalyzer = new();
            CircularDependencyDetector _circularDependencyDetector = new();
            SlnxParser _slnxParser = new();
            var scanner = new SolutionScanner(_projectParser, _serviceDetector, _communicationAnalyzer, _circularDependencyDetector, _slnxParser, consoleService);
            var result = await scanner.AnalyzeSolutionAsync(command.SolutionPath);

            consoleService.DisplaySuccess("Analysis complete!");
            consoleService.DisplayInfo($"  - Projects: {result.Statistics.TotalProjects}");
            consoleService.DisplayInfo($"  - NuGet Packages: {result.Statistics.TotalNuGetPackages}");
            consoleService.DisplayInfo($"  - Services: {result.Statistics.TotalServices}");
            consoleService.DisplayInfo($"  - Duration: {result.Statistics.AnalysisDuration.TotalSeconds:F2}s");

            if (result.CircularDependencies.Any())
            {
                consoleService.DisplayWarning($"Circular Dependencies: {result.CircularDependencies.Count}");
            }

            consoleService.DisplayLineSeparator();

            if(OutputFormatTypesHelper.IsHtmlFormat(command.OutputFormat) ||
                OutputFormatTypesHelper.IsBothFormat(command.OutputFormat))
            {
                HtmlReportGenerator htmlReportGenerator = new();
                var htmlPath = command.OutputPath.EndsWith(".html")
                    ? command.OutputPath
                    : Path.ChangeExtension(command.OutputPath, ".html");
                var generatedHtmlPath = htmlReportGenerator.GenerateReport(result, htmlPath);

                Console.ForegroundColor = ConsoleColor.Cyan;
                string htmlRelativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), generatedHtmlPath);
                consoleService.DisplaySuccess($"HTML report generated at: {htmlRelativePath}");
            }

            consoleService.DisplayInfo("Done");
            return 0;
        }
        catch (Exception ex)
        {
            consoleService.DisplayErrorWithStackTrace(ex.Message, ex.StackTrace ?? string.Empty);
            throw;
        }
    }
}
