namespace Clarity.Dev.NET.Analyzer.Core;

public class SolutionScanner
{
    /// <summary>
    /// Analyzes a .NET solution (.sln or .slnx) and returns comprehensive analysis results
    /// </summary>
    /// <param name="solutionPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<SolutionModels.SolutionAnalysisResult> AnalyzeSolutionAsync(
        string solutionPath,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if(!File.Exists(solutionPath))
        {
            throw new FileNotFoundException("Solution file not found.", solutionPath);
        }

        var result = new SolutionModels.SolutionAnalysisResult
        {
            SolutionName = Path.GetFileNameWithoutExtension(solutionPath),
            SolutionPath = solutionPath,
            AnalyzedAt = DateTime.UtcNow
        };

        Console.WriteLine($"Analyzing Solution: {result.SolutionName}");

        // Step 1: Detect solution file type and load projects accordingly
        var extension = Path.GetExtension(solutionPath).ToLowerInvariant();
        AdhocWorkspace workspace;
        AnalyzerManager? manager = null;

        if (SolutionExtensionTypeHelper.IsSlnxFile(extension))
        {
            // Parse .slnx file manually and create workspace
            Console.WriteLine("Detexted .slnx file (XML-based solution)");
            throw new NotImplementedException(".slnx file analysis is not yet implemented.");
        }
        else if(SolutionExtensionTypeHelper.IsSlnFile(extension))
        {
            // Use Buildalyze to scan traditional .sln file
            Console.WriteLine("Detected .sln file (traditional solution)");
            manager = new(solutionPath);
            workspace = manager.GetWorkspace();
        }
        else
        {
            throw new NotSupportedException("Unsupported solution file type.");
        }

        Console.WriteLine($"Found {workspace.CurrentSolution.Projects.Count()} projects");

        // Step 2: Parse and analyze each project in solution in parallel

        // Step 3: Detect services in each project

        // Step 4: Analyze service communication

        // Step 5: Detext circular dependencies

        // Step 6: Calculate statistics

        stopwatch.Stop();

        return result;
    }
}
