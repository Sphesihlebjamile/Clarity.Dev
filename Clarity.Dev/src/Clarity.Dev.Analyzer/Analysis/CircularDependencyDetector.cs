namespace Clarity.Dev.NET.Analyzer.Analysis;

/// <summary>
/// Detects circular dependencies between projects 🦢
/// </summary>
public class CircularDependencyDetector: ICircularDependencyDetector
{
    public List<SolutionModels.CircularDependency> DetectCircularDependencies(List<SolutionModels.ProjectInfo> projects)
    {
        List<SolutionModels.CircularDependency> circularDependencies = [];
        HashSet<string> visited = [];
        Dictionary<string, SolutionModels.ProjectInfo> projectMap = projects.ToDictionary(p => p.Name, p => p);

        ArgumentNullException.ThrowIfNull(projects, nameof(projects));

        foreach (var project in projects)
        {
            if (!visited.Contains(project.Name))
            {
                DetectCyclesRec(
                    project.Name,
                    projectMap,
                    visited,
                    new HashSet<string>(),
                    new List<string>(),
                    circularDependencies);
            }
        }

        return circularDependencies;
    }

    /// <summary>
    /// Recursively explores project references to detect circular dependencies within a set of projects 🦢
    /// </summary>
    /// <remarks>
    /// This method uses a depth-first search approach to traverse project references. When a project
    /// is encountered that is already in the current recursion stack, a circular dependency is identified and recorded.
    /// Duplicate cycles are not added to the results.
    /// </remarks>
    /// <param name="projectName">The name of the project to analyze for circular dependencies.</param>
    /// <param name="projectMap">A dictionary that maps project names to their corresponding project information, including referenced projects.</param>
    /// <param name="visited">A set containing the names of projects that have already been visited during the cycle detection process.</param>
    /// <param name="recursionStack">A set of project names representing the current recursion path, used to identify cycles.</param>
    /// <param name="currentPath">A list representing the sequence of project names in the current traversal path.</param>
    /// <param name="circularDependencies">A list that accumulates detected circular dependencies, each represented as a project path.</param>
    private void DetectCyclesRec(
        string projectName,
        Dictionary<string, SolutionModels.ProjectInfo> projectMap,
        HashSet<string> visited,
        HashSet<string> recursionStack,
        List<string> currentPath,
        List<SolutionModels.CircularDependency> circularDependencies)
    {
        // Mark as visited and add to current recursion path
        visited.Add(projectName);
        recursionStack.Add(projectName);
        currentPath.Add(projectName);

        if(projectMap.TryGetValue(projectName, out var project))
        {
            foreach(var dependency in project.ProjectReferences)
            {
                if (!visited.Contains(dependency))
                {
                    // Not yet explored, recurse
                    DetectCyclesRec(
                        dependency,
                        projectMap,
                        visited,
                        recursionStack,
                        currentPath,
                        circularDependencies);
                } else if (recursionStack.Contains(dependency))
                {
                    // Found a back edge, this is a cycle.
                    var cycleStartIndex = currentPath.IndexOf(dependency);
                    var cyclePath = new List<string>(currentPath.Skip(cycleStartIndex));

                    // only add if not duplicate cycle
                    if(!IsCycleDuplicate(cyclePath, circularDependencies))
                    {
                        circularDependencies.Add(new()
                        {
                            ProjectPath = cyclePath
                        });
                    }
                }
            }
        }

        // Backtrack: remove from current path and recursion stack
        currentPath.RemoveAt(currentPath.Count - 1);
        recursionStack.Remove(projectName);
    }

    /// <summary>
    /// Determines whether the specified cycle is a duplicate of any existing cycles, considering all possible
    /// rotations 🦢
    /// </summary>
    /// <remarks>
    /// The method checks for duplicates by comparing the new cycle against each existing cycle,
    /// allowing for different rotations of the cycle.
    /// </remarks>
    /// <param name="newCycle">A list of strings representing the new cycle to check for duplicates.</param>
    /// <param name="existing">A list of existing CircularDependency objects that represent previously recorded cycles.</param>
    /// <returns>true if the new cycle is a duplicate of any existing cycle; otherwise, false.</returns>
    private bool IsCycleDuplicate(List<string> newCycle, List<SolutionModels.CircularDependency> existing)
    {
        foreach (var existingDep in existing)
        {
            if (existingDep.ProjectPath.Count != newCycle.Count)
                continue;

            // Check if the cycle matches at any rotation
            for(int rotation = 0; rotation < newCycle.Count; rotation++)
            {
                bool isMatch = true;
                for (int i = 0; i < newCycle.Count; i++)
                {
                    if (existingDep.ProjectPath[i] != newCycle[(i + rotation) % newCycle.Count])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                    return true;
            }
        }
        return false;
    }
}
