namespace Clarity.Dev.NET.Analyzer.Analysis;

/// <summary>
/// Detects various types of services in a .NET project 🦢
/// </summary>
public class ServiceDetector
{
    public async Task<List<ServiceInfo>> DetectServicesAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        var services = new List<ServiceInfo>();

        foreach(var document in project.Documents)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            // Get Syntax Tree

            // Get Semantic Model

            // Detect Controllers

            // Detect Background Services

            // Detect SignalR hubs

            // Detext gRPC services

            // Detect Minimal APIs (from Program.cs)
        }

        return services;
    }
}
