namespace Clarity.Dev.NET.Analyzer.Core;

/// <summary>
/// Main orchestrator for analyzing .NET solutions 🦢
/// </summary>
/// <param name="projectParser"></param>
public class SolutionScanner(
    IProjectParser projectParser,
    IServiceDetector serviceDetector,
    ICommunicationAnalyzer communicationAnalyzer,
    ICircularDependencyDetector circularDependencyDetector,
    ISlnxParser slnxParser,
    IConsoleService consoleService) : ISolutionScanner
{
    private readonly IProjectParser _projectParser = projectParser;
    private readonly IServiceDetector _serviceDetector = serviceDetector;
    private readonly ICommunicationAnalyzer _communicationAnalyzer = communicationAnalyzer;
    private readonly ICircularDependencyDetector _circularDependencyDetector = circularDependencyDetector;
    private readonly ISlnxParser _slnxParser = slnxParser;
    private readonly IConsoleService _consoleService = consoleService;

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
            workspace = await LoadSlnxSolutionAsync(solutionPath, _slnxParser);
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
        var projectTasks = workspace.CurrentSolution.Projects
            .Select(async project =>
            {
                try
                {
                    return await _projectParser.ParseProjectAsync(project, manager, cancellationToken);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error parsing project {project.Name}: {ex.Message}");
                    return null;
                }
            });

        var parsedProjects = await Task.WhenAll(projectTasks);
        result.Projects = parsedProjects.Where(proj => proj is not null)
            .Cast<SolutionModels.ProjectInfo>()
            .ToList();

        Console.WriteLine($"Successfully parsed {result.Projects.Count} projects!");

        // Step 3: Detect services in each project
        foreach(var projectInfo in result.Projects)
        {
            var roslynProject = workspace.CurrentSolution.Projects.FirstOrDefault(proj => proj.Name == projectInfo.Name);
            if(roslynProject is not null)
            {
                projectInfo.DetectedServices = await _serviceDetector.DetectServicesAsync(
                    roslynProject,
                    cancellationToken);
            }
        }

        Console.WriteLine($"Detected {result.Projects.Sum(p => p.DetectedServices.Count)} services");

        // Step 4: Analyze service communication
        result.ServiceCommunications = await _communicationAnalyzer.AnalyzeCommunicationAsync(
            workspace.CurrentSolution,
            result.Projects,
            cancellationToken);

        // Step 5: Detect circular dependencies
        result.CircularDependencies = _circularDependencyDetector.DetectCircularDependencies(result.Projects);

        if (result.CircularDependencies.Any())
        {
            Console.WriteLine($"⚠️  WARNING: Found {result.CircularDependencies.Count} circular dependencies!");
        }

        // Step 6: Calculate 
        stopwatch.Stop();

        result.Statistics = new SolutionModels.AnalysisStatistics
        {
            TotalProjects = result.Projects.Count,
            TotalNuGetPackages = result.Projects.Sum(project => project.NuGetDependencies.Count),
            TotalProjectReferences = result.Projects.Sum(project => project.ProjectReferences.Count),
            TotalServices = result.Projects.Sum(project => project.DetectedServices.Count),
            CircularDependencyCount = result.CircularDependencies.Count,
            AnalysisDuration = stopwatch.Elapsed
        };

        return result;
    }

    /// <summary>
    /// Loads a .slnx file and creates a Roslyn workspace 🦢
    /// </summary>
    /// <param name="slnxPath">Path to .slnx file</param>
    /// <returns></returns>
    private async Task<AdhocWorkspace> LoadSlnxSolutionAsync(string slnxPath, ISlnxParser slnxParser)
    {
        var workspace = new AdhocWorkspace();
        var projectPaths = slnxParser.ParseSlnx(slnxPath);

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
