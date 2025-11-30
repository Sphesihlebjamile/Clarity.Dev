namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Represents a NuGet dependency 🦢
/// </summary>
public class DependencyInfo
{
    public string PackageName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsDirectDependency { get; set; } = true;
}
