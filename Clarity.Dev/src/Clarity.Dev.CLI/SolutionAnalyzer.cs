namespace Clarity.Dev.CLI;

internal static class SolutionAnalyzer
{
    public static async Task<int> AnalyzeSolution(SolutionAnalysisInput solutionAnalysisInput)
    {
        try
        {
            if (!File.Exists(solutionAnalysisInput.SolutionPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: Solution file not found: {solutionAnalysisInput.SolutionPath}");
                Console.ResetColor();
                return 1;
            }

            ProjectParser _projectParser = new();
            ServiceDetector _serviceDetector = new();
            CommunicationAnalyzer _communicationAnalyzer = new();
            CircularDependencyDetector _circularDependencyDetector = new();
            var scanner = new SolutionScanner(_projectParser, _serviceDetector, _communicationAnalyzer, _circularDependencyDetector);
            var result = await scanner.AnalyzeSolutionAsync(solutionAnalysisInput.SolutionPath);

            Console.WriteLine();
            Console.WriteLine("=======================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Analysis complete!");
            Console.ResetColor();
            Console.WriteLine($"  • Projects: {result.Statistics.TotalProjects}");
            Console.WriteLine($"  • NuGet Packages: {result.Statistics.TotalNuGetPackages}");
            Console.WriteLine($"  • Services: {result.Statistics.TotalServices}");
            Console.WriteLine($"  • Duration: {result.Statistics.AnalysisDuration.TotalSeconds:F2}s");

            if (result.CircularDependencies.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  ⚠️  Circular Dependencies: {result.CircularDependencies.Count}");
                Console.ResetColor();
            }

            Console.WriteLine("=======================================");
            Console.WriteLine();

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
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
                if (!OutputFormatTypesHelper.IsValidOutputFormat(arguments[inputIndex]))
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
