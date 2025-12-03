namespace Clarity.Dev.NET.Analyzer.Core;

/// <summary>
/// Parses individual project files and extracts metadata.
/// </summary>
public class ProjectParser
{
    public async Task<SolutionModels.ProjectInfo> ParseProjectAsync(
        Project project,
        AnalyzerManager? manager,
        CancellationToken cancellationToken = default)
    {
        var projectInfo = new SolutionModels.ProjectInfo
        {
            Name = project.Name,
            Path = project.FilePath ?? string.Empty,
        };

        // Use Buildalyzer if available (for .sln file)
        if (manager is not null && !string.IsNullOrEmpty(project.FilePath))
        {
            var analyzer = manager.GetProject(project.FilePath);
            var analyzerResults = analyzer.Build();

            if(analyzerResults is IAnalyzerResults && analyzerResults.Any())
            {
                var firstResult = analyzerResults.First();

                // Extract target framework
                projectInfo.TargetFramework = firstResult.GetProperty("LangVersion") ?? "Default";

                // Extract output type
                projectInfo.OutputType = firstResult.GetProperty("OutputType") ?? "Library";

                // Extract C# language version
                projectInfo.CSharpVersion = firstResult.GetProperty("LangVersion") ?? "Default";

                // Extract NuGet package references
                var packageReferences = firstResult.PackageReferences;
                if(packageReferences is IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> references)
                {
                    foreach(var reference in references)
                    {
                        projectInfo.NuGetDependencies.Add(new SolutionModels.DependencyInfo()
                        {
                            PackageName = reference.Key,
                            Version = reference.Value.ContainsKey("Version") ? reference.Value["Version"] : "Unknown",
                            IsDirectDependency = true
                        });
                    }
                }

                // Extract project references
                var projectReferences = firstResult.ProjectReferences;
                foreach(var projRef in projectReferences)
                {
                    var refName = Path.GetFileNameWithoutExtension(projRef);
                    projectInfo.ProjectReferences.Add(refName);
                }
            }
        }
        // For .slnx files, parse the project directly
        else if (!string.IsNullOrEmpty(project.FilePath))
        {
        }

        return await Task.FromResult(projectInfo);
    }
}
