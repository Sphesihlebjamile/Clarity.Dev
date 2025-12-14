namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Represents a single project in the solution 🦢
/// </summary>
public class ProjectInfo
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string TargetFramework { get; set; } = string.Empty;
    public string CSharpVersion { get; set; } = string.Empty;
    public string OutputType { get; set; } = string.Empty; // Exe, Library, WinExe, etc.
    public List<DependencyInfo> NuGetDependencies { get; set; } = [];
    public List<string> ProjectReferences { get; set; } = [];
    public List<ServiceInfo> DetectedServices { get; set; } = new();
    public bool IsTestProject { get; set; }
}
