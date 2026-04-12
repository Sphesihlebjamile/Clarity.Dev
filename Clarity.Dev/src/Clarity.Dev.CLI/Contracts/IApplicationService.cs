namespace Clarity.Dev.CLI.Contracts;

public interface IApplicationService
{
    Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default);
}
