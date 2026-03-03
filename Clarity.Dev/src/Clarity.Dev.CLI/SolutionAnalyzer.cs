using Clarity.Dev.NET.Analyzer.Contracts;

namespace Clarity.Dev.CLI;

internal static class SolutionAnalyzer
{
    public static async Task<int> AnalyzeSolution(SolutionAnalysisInput solutionAnalysisInput, IConsoleService consoleService)
    {
        try
        {

            if (!File.Exists(solutionAnalysisInput.SolutionPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                consoleService.DisplayInfo($"❌ Error: Solution file not found: {solutionAnalysisInput.SolutionPath}");
                Console.ResetColor();
                return 1;
            }

            ProjectParser _projectParser = new();
            ServiceDetector _serviceDetector = new();
            CommunicationAnalyzer _communicationAnalyzer = new();
            CircularDependencyDetector _circularDependencyDetector = new();
            SlnxParser _slnxParser = new();
            var scanner = new SolutionScanner(_projectParser, _serviceDetector, _communicationAnalyzer, _circularDependencyDetector, _slnxParser, consoleService);
            var result = await scanner.AnalyzeSolutionAsync(solutionAnalysisInput.SolutionPath);

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

            if(OutputFormatTypesHelper.IsHtmlFormat(solutionAnalysisInput.OutputFormat) ||
                OutputFormatTypesHelper.IsBothFormat(solutionAnalysisInput.OutputFormat))
            {
                HtmlReportGenerator htmlReportGenerator = new();
                var htmlPath = solutionAnalysisInput.OutputPath.EndsWith(".html")
                    ? solutionAnalysisInput.OutputPath
                    : Path.ChangeExtension(solutionAnalysisInput.OutputPath, ".html");
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

    public static SolutionAnalysisInput GetOutputCommands(string[] arguments)
    {
        SolutionAnalysisInput input = new();

        for (int inputIndex = 0; inputIndex < arguments.Length; inputIndex++)
        {
            if (inputIndex == 0 &&
                !SolutionCommandConstantsHelper.IsOutputOption(arguments[inputIndex]) &&
                !SolutionCommandConstantsHelper.IsOutputPathOption(arguments[inputIndex]) &&
                !SolutionCommandConstantsHelper.IsPathOption(arguments[inputIndex]))
            {
                input.SolutionPath = arguments[0];
            }
            else if (SolutionCommandConstantsHelper.IsOutputOption(arguments[inputIndex]) && (inputIndex + 1) <= arguments.Length)
            {
                if (!OutputFormatTypesHelper.IsValidOutputFormat(arguments[inputIndex + 1]))
                {
                    throw new Exception("Invalid Output Format!");
                }
                if(File.Exists(arguments[inputIndex + 1]))
                {
                    throw new Exception("Output file already exists!");
                }
                if (!string.IsNullOrEmpty(input.OutputFormat))
                {
                    // The output format has already been set, skip to avoid overwriting
                    continue;
                }
                input.OutputFormat = arguments[inputIndex + 1];
                inputIndex++;
            }
            else if (SolutionCommandConstantsHelper.IsOutputPathOption(arguments[inputIndex]) && (inputIndex + 1) <= arguments.Length)
            {
                if (!string.IsNullOrEmpty(input.OutputPath))
                {
                    // The output path has already been set, skip to avoid overwriting
                    continue;
                }
                input.OutputPath = arguments[inputIndex + 1];
                inputIndex++;
            }
            else if (SolutionCommandConstantsHelper.IsPathOption(arguments[inputIndex]) && (inputIndex + 1) <= arguments.Length)
            {
                if (!string.IsNullOrEmpty(input.SolutionPath))
                {
                    // The output path has already been set, skip to avoid overwriting
                    continue;
                }
                input.SolutionPath = arguments[inputIndex + 1];
                inputIndex++;
            }
            else
            {
                throw new Exception($"Unrecognized argument: {arguments[inputIndex]}");
            }
        }

        // If the solution path is not provided, we can attempt to find a .sln file in the current directory
        if (string.IsNullOrEmpty(input.SolutionPath))
        {
            // If we're on debug, we can look for the default solution path in the appsettings.json file, otherwise set the current directory as the solution path
            #if DEBUG
                var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
                input.SolutionPath = config.GetSection("DefaultTestProject").Value ?? GetDefaultSolutionPath();
            #else
                input.SolutionPath = GetDefaultSolutionPath();   
            #endif
        }
        if (string.IsNullOrEmpty(input.OutputPath))
        {
            input.OutputPath = GetDefaultOutputPath();
        }
        if (string.IsNullOrEmpty(input.OutputFormat))
        {
            input.OutputFormat = GetDefaultOutputFormat();
        }
        return input;
    }

    private static string GetDefaultSolutionPath() => Directory.GetCurrentDirectory();

    private static string GetDefaultOutputPath() => Path.Combine(Directory.GetCurrentDirectory(), "Clarity.Dev.Output");

    private static string GetDefaultOutputFormat() => OutputFormatTypes.html.ToString();
}
