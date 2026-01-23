namespace Clarity.Dev.NET.Analyzer.Core;

/// <summary>
/// Parses individual project files and extracts metadata 🦢
/// </summary>
public class ProjectParser
{
    /// <summary>
    /// Parses a Roslyn project and extracts all relevant information 🦢
    /// </summary>
    /// <param name="project"></param>
    /// <param name="manager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
            ParseSlnxProject(projectInfo, project.FilePath);
        }

        // Detect if this is a test project
        projectInfo.IsTestProject = IsTestProject(projectInfo);

        return await Task.FromResult(projectInfo);
    }

    /// <summary>
    /// Parses a .xlnx file 🦢
    /// </summary>
    /// <param name="projectInfo"></param>
    /// <param name="projectPath"></param>
    private static void ParseSlnxProject(SolutionModels.ProjectInfo projectInfo, string projectPath)
    {
        try
        {
            var xdocument = XDocument.Load(projectPath);
            var xnamespace = xdocument.Root?.Name?.Namespace ?? XNamespace.None;

            if (xdocument is null || xnamespace is null)
                return;

            // Extract target framework
            var targetFramework = xdocument.Descendants(xnamespace + "TargetFramework").FirstOrDefault()?.Value
                ?? xdocument?.Descendants(xnamespace + "TargetFramework").FirstOrDefault()?.Value?.Split(";").FirstOrDefault()
                ?? "Unknown";
            projectInfo.TargetFramework = targetFramework;

            // Extract C# langauge version
            projectInfo.CSharpVersion = xdocument.Descendants(xnamespace + "LangVersion").FirstOrDefault()?.Value ?? "Default";

            // Extract output type
            projectInfo.OutputType = xdocument.Descendants(xnamespace + "OutputType").FirstOrDefault()?.Value ?? "Library";

            // Extract NuGet package references
            var packageRefs = xdocument.Descendants(xnamespace + "PackageReference");
            foreach (var packageRef in packageRefs)
            {
                var packageName = packageRef.Attribute("Include")?.Value;
                var version = packageRef.Attribute("Version")?.Value
                    ?? packageRef.Element(xnamespace + "Version")?.Value
                    ?? "Unknown";

                if (!string.IsNullOrEmpty(packageName))
                {
                    projectInfo.NuGetDependencies.Add(new DependencyInfo
                    {
                        PackageName = packageName,
                        Version = version,
                        IsDirectDependency = true
                    });
                }
            }

            // Extract project references
            var projectRefs = xdocument.Descendants(xnamespace + "ProjectReference");
            foreach (var projRef in projectRefs)
            {
                var includePath = projRef.Attribute("Include")?.Value;
                if (!string.IsNullOrEmpty(includePath))
                {
                    var refName = Path.GetFileNameWithoutExtension(includePath);
                    projectInfo.ProjectReferences.Add(refName);
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Warning: Error parsing project file {projectPath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Determines if a project is a test project based on naming and dependencies 🦢
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private static bool IsTestProject(SolutionModels.ProjectInfo project)
    {
        // Validate project name
        if (project.Name.Contains("Test", StringComparison.OrdinalIgnoreCase) ||
            project.Name.Contains("Spec", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check for common test framework dependencies
        var testFrameworks = new List<string>()
        {
            "xunit", "nunit", "mstest", "xunit.core"
        };

        return project.NuGetDependencies.Any(dependency =>
            testFrameworks.Any(testFramework => dependency.PackageName.Contains(testFramework, StringComparison.OrdinalIgnoreCase)));
    }
}
