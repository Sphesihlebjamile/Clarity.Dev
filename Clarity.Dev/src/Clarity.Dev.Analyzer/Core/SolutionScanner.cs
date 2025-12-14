namespace Clarity.Dev.NET.Analyzer.Core;

/// <summary>
/// Main orchestrator for analyzing .NET solutions 🦢
/// </summary>
/// <param name="projectParser"></param>
public class SolutionScanner(ProjectParser projectParser)
{
    private readonly ProjectParser _projectParser = projectParser;

    /// <summary>
    /// Analyzes a .NET solution (.sln or .slnx) and returns comprehensive analysis results 🦢
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
            workspace = await LoadSlnxSolutionAsync(solutionPath);
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

    /// <summary>
    /// Loads a .slnx file and creates a Roslyn workspace 🦢
    /// </summary>
    /// <param name="slnxPath">Path to .slnx file</param>
    /// <returns></returns>
    private async Task<AdhocWorkspace> LoadSlnxSolutionAsync(string slnxPath)
    {
        var workspace = new AdhocWorkspace();
        var projectPaths = SlnxParser.ParseSlnx(slnxPath);

        foreach(var projectPath in projectPaths)
        {
            try
            {
                var projectName = Path.GetFileNameWithoutExtension(projectPath);
                var projectId = ProjectId.CreateNewId(projectName);

                // Load project file to get language (C#, F#, VB)
                var languageName = GetLanguageFromProjectFile(projectPath);

                var projectInfo = Microsoft.CodeAnalysis.ProjectInfo.Create(
                    projectId,
                    VersionStamp.Default,
                    projectName,
                    projectName,
                    languageName,
                    filePath: projectPath
                    );

                workspace.AddProject(projectInfo);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Warning: Could not load project{projectPath}: {ex.Message}");
            }
        }

        return await Task.FromResult(workspace);
    }

    /// <summary>
    /// Determines the language from a project file extension 🦢
    /// </summary>
    /// <param name="projectPath"></param>
    /// <returns></returns>
    private string GetLanguageFromProjectFile(string projectPath)
    {
        var extension = Path.GetExtension(projectPath).ToLowerInvariant();
        return extension switch
        {
            var e when ProjectExtensionTypeHandler.IsCsproj(e) => LanguageNames.CSharp,
            var e when ProjectExtensionTypeHandler.IsFsproj(e) => LanguageNames.FSharp,
            var e when ProjectExtensionTypeHandler.IsVbproj(e) => LanguageNames.VisualBasic,
            _ => LanguageNames.CSharp
        };
    }
}
