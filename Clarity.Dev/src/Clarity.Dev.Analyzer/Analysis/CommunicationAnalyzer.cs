
namespace Clarity.Dev.NET.Analyzer.Analysis;

/// <summary>
/// Analyzes inter-service communication patterns 🦢
/// </summary>
public class CommunicationAnalyzer
{
    public async Task<List<CommunicationPath>> AnalyzeCommunicationAsync(
        Solution solution,
        List<SolutionModels.ProjectInfo> projects,
        CancellationToken cancellationToken = default)
    {
        List<CommunicationPath> communications = [];

        foreach(var project in solution.Projects)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            foreach(var document in project.Documents)
            {
                var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
                if (syntaxTree is null) continue;

                var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                if (semanticModel is null) continue;

                var root = await syntaxTree.GetRootAsync(cancellationToken);

                // Detect HTTP client usage
                communications.AddRange(DetectHttpClients(root, semanticModel, project.Name));

                // Detect gRPC client usage
                communications.AddRange(DetectGrpcClients(root, semanticModel, project.Name));

                // Detect message queue usage
                communications.AddRange(DetectMessageQueues(root, semanticModel, project.Name));

                // Detect database access
                communications.AddRange(DetectDatabaseAccess(root, semanticModel, project.Name));
            }
        }

        return communications;
    }

    private List<CommunicationPath> DetectGrpcClients(SyntaxNode root, SemanticModel semanticModel, string name)
    {
        throw new NotImplementedException($"Have not implemented the {nameof(DetectGrpcClients)} function");
    }

    private List<CommunicationPath> DetectMessageQueues(SyntaxNode root, SemanticModel semanticModel, string name)
    {
        throw new NotImplementedException($"Have not implemented the {nameof(DetectMessageQueues)} function");
    }

    private List<CommunicationPath> DetectDatabaseAccess(SyntaxNode root, SemanticModel semanticModel, string name)
    {
        throw new NotImplementedException($"Have not implemented the {nameof(DetectDatabaseAccess)} function");
    }

    private List<CommunicationPath> DetectHttpClients(SyntaxNode root, SemanticModel semanticModel, string projectName)
    {
        throw new NotImplementedException($"Have not implemented the {nameof(DetectHttpClients)} function");
    }
}
