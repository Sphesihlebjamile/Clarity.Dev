namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Represents communication between services or to external systems 🦢
/// </summary>
public class CommunicationPath
{
    public string SourceProject { get; set; } = string.Empty;
    public string TargetService { get; set; } = string.Empty;
    public required CommunicationType Type { get; set; }
    public string? Endpoint { get; set; }
}
