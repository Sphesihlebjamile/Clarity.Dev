namespace Clarity.Dev.CLI;

internal static class SolutionAnalyzer
{
    public static async Task<int> AnalyzeSolution(string solutionPath)
    {
        try
        {
            if (!File.Exists(solutionPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: Solution file not found: {solutionPath}");
                Console.ResetColor();
                return 1;
            }

            ProjectParser _projectParser = new();
            ServiceDetector _serviceDetector = new();
            CommunicationAnalyzer _communicationAnalyzer = new();
            CircularDependencyDetector _circularDependencyDetector = new();
            var scanner = new SolutionScanner(_projectParser, _serviceDetector, _communicationAnalyzer, _circularDependencyDetector);
            var result = await scanner.AnalyzeSolutionAsync(solutionPath);

            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════");
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

            Console.WriteLine("═══════════════════════════════════════");
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
            return 1;
        }
    }
}
