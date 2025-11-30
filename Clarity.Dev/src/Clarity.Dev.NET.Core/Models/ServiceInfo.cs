namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Represents a detected service, controller, or background service 🦢
/// </summary>
public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public required ServiceType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public List<string> Routes { get; set; } = [];
    public List<string> Dependencies { get; set; } = [];
}
