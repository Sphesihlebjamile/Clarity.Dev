
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

    /// <summary>
    /// Analyzes the provided syntax tree to detect whether the project accesses a database using common patterns such
    /// as Entity Framework or Dapper 🦢
    /// </summary>
    /// <remarks>
    /// This method identifies database access by searching for classes that inherit from DbContext
    /// (Entity Framework) or for usage of Dapper/ADO.NET-related identifiers. Only one communication path per type of
    /// database access is added.
    /// </remarks>
    /// <param name="root">The root syntax node of the C# source code to analyze.</param>
    /// <param name="semanticModel">The semantic model associated with the syntax tree, used to resolve type information.</param>
    /// <param name="projectName">The name of the project being analyzed. Used to identify the source of detected communication paths.</param>
    /// <returns>
    /// A list of communication paths representing detected database access patterns in the project. The list is empty
    /// if no database access is found.
    /// </returns>
    private List<CommunicationPath> DetectDatabaseAccess(SyntaxNode root, SemanticModel semanticModel, string projectName)
    {
        List<CommunicationPath> communicationPaths = [];
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach(var classDeclaration in classDeclarations)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
            if(symbol == null) continue;

            // Entity Framework DbContext
            if(InheritsFrom(symbol, "DbContext"))
            {
                communicationPaths.Add(new CommunicationPath
                {
                    SourceProject = projectName,
                    TargetService = "Database",
                    Type = CommunicationType.Database,
                });
                break;
            }
        }

        // Dapper usage
        var identifier = root.DescendantNodes().OfType<IdentifierNameSyntax>()
            .Where(ins => ins.Identifier.ValueText.Contains("Dapper") ||
                ins.Identifier.ValueText.Equals("Query") ||
                ins.Identifier.ValueText.Equals("Execute"));

        if (identifier.Any())
        {
            if(!communicationPaths.Any(path => path.Type == CommunicationType.Database))
            {
                communicationPaths.Add(new CommunicationPath
                {
                    SourceProject = projectName,
                    TargetService = "Database (Dapper/ADO.NET)",
                    Type = CommunicationType.Database
                });
            }
        }

        return communicationPaths;
    }

    /// <summary>
    /// Detects usages of HttpClient or related HTTP client factories within the specified syntax tree 🦢
    /// </summary>
    /// <remarks>
    /// This method identifies both direct instantiations of HttpClient and references to HTTP client
    /// factories, such as IHttpClientFactory. Only one communication path is recorded per project, even if multiple
    /// usages are found.
    /// </remarks>
    /// <param name="root">The root syntax node of the code to analyze for HTTP client usage.</param>
    /// <param name="semanticModel">The semantic model used to provide type information for the syntax nodes.</param>
    /// <param name="projectName">The name of the project being analyzed. Used to identify the source of detected communication paths.</param>
    /// <returns>
    /// A list of communication paths representing detected HTTP client usages in the project. The list is empty if no
    /// HTTP client usage is found.
    /// </returns>
    private List<CommunicationPath> DetectHttpClients(SyntaxNode root, SemanticModel semanticModel, string projectName)
    {
        List<CommunicationPath> communicationPaths = [];

        // Look for HttpClient instantiation or usage
        var objectCreations = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();

        foreach(var creation in objectCreations)
        {
            var typeInfo = semanticModel.GetTypeInfo(creation);
            if(typeInfo.Type?.Name == "HttpClient")
            {
                communicationPaths.Add(new CommunicationPath
                {
                    SourceProject = projectName,
                    TargetService = "External HTTP Service",
                    Type = CommunicationType.HttpClient,
                });
                break; // Only record once per project
            }
        }

        // Also loog for IHttpClientFactory usage
        var identifier = root.DescendantNodes().OfType<IdentifierNameSyntax>()
            .Where(ins => ins.Identifier.ValueText.Contains("HttpClient"));

        if (identifier.Any())
        {
            communicationPaths.Add(new CommunicationPath
            {
                SourceProject = projectName,
                TargetService = "External HTTP Service",
                Type = CommunicationType.HttpClient,
            });
        }

        return communicationPaths;
    }

    /// <summary>
    /// Determines whether the specified type symbol inherits from a base type with the given name 🦢
    /// </summary>
    /// <param name="symbol">The type symbol to examine for inheritance.</param>
    /// <param name="baseTypeName">The name of the base type to check for in the inheritance hierarchy.</param>
    /// <returns>
    /// true if the specified type symbol inherits from a base type with the given name; otherwise, false.
    /// </returns>
    private bool InheritsFrom(INamedTypeSymbol symbol, string baseTypeName)
    {
        var current = symbol.BaseType;
        while(current != null)
        {
            if(current.Name == baseTypeName)
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }
}
