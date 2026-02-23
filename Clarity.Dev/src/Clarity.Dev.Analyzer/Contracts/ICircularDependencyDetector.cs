namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Detects circular dependencies between projects 🦢
/// </summary>
public interface ICircularDependencyDetector
{
    /// <summary>
    /// Analyzes the specified projects to detect circular dependencies within the project set 🦢
    /// </summary>
    /// <remarks>
    /// This method traverses the project dependency graph using a depth-first search to identify
    /// cycles. Each project is visited only once to optimize performance.
    /// </remarks>
    /// <param name="projects">A list of project information objects representing the projects to analyze for circular dependencies. Cannot be
    /// null.
    /// </param>
    /// <returns>
    /// A list of CircularDependency objects, each representing a detected circular dependency among the provided
    /// projects. The list is empty if no circular dependencies are found.
    /// </returns>
    public List<SolutionModels.CircularDependency> DetectCircularDependencies(List<SolutionModels.ProjectInfo> projects);
}
