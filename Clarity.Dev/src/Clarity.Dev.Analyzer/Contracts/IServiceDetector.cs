namespace Clarity.Dev.NET.Analyzer.Contracts;

/// <summary>
/// Detects various types of services in a .NET project 🦢
/// </summary>
public interface IServiceDetector
{
    /// <summary>
    /// Asynchronously detects and returns information about services defined within the specified project, including
    /// controllers, background services, SignalR hubs, gRPC services, and minimal APIs 🦢
    /// </summary>
    /// <remarks>This method scans all documents in the provided project to identify various types of services
    /// commonly used in .NET applications. The operation can be cancelled by passing a cancellation token.</remarks>
    /// <param name="project">The project to analyze for service definitions. Must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A list of ServiceInfo objects representing the detected services in the project. The list is empty if no
    /// services are found.</returns>
    public Task<List<ServiceInfo>> DetectServicesAsync(
        Project project,
        CancellationToken cancellationToken = default);
}
