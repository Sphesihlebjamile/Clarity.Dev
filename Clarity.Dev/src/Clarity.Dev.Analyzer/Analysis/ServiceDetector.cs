using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            if (syntaxTree is null) continue;

            // Get Semantic Model
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            if(semanticModel is null) continue;

            // Get Root Node
            var root = await syntaxTree.GetRootAsync(cancellationToken);
            if(root is null) continue;

            // Detect Controllers
            services.AddRange(DetectControllers(root, semanticModel, document.FilePath ?? string.Empty));

            // Detect Background Services
            services.AddRange(DetectBackgroundServices(root, semanticModel, document.FilePath ?? string.Empty));

            // Detect SignalR hubs
            services.AddRange(DetectSignalRHubs(root, semanticModel, document.FilePath ?? string.Empty));

            // Detext gRPC services
            services.AddRange(DetectGrpcServices(root, semanticModel, document.FilePath ?? string.Empty));

            // Detect Minimal APIs (from Program.cs)
            if(document.Name == "Program.cs")
            {
                services.AddRange(DetectMinimalApis(root, document.FilePath ?? string.Empty));
            }
        }

        return services;
    }

    private List<ServiceInfo> DetectControllers(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        throw new NotImplementedException($"Have not implemented the {nameof(DetectControllers)} function!");
    }

    private List<ServiceInfo> DetectBackgroundServices(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        throw new NotImplementedException($"Have not implemented the {DetectBackgroundServices} function!");
    }

    private List<ServiceInfo> DetectSignalRHubs(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        throw new NotImplementedException($"Have not implemented the {DetectSignalRHubs} function!");
    }

    private List<ServiceInfo> DetectGrpcServices(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        throw new NotImplementedException($"Have not implemented the {DetectGrpcServices} function!");
    }

    private List<ServiceInfo> DetectMinimalApis(SyntaxNode root, string filePath)
    {
        throw new NotImplementedException($"Have not implemented the {DetectMinimalApis} function!");
    }

    /// <summary>
    /// Extracts all HTTP routes from a controller class by combining class-level and method-level route attributes. 🦢
    /// </summary>
    /// <param name="classDeclarationSyntax">The class declaration syntax to extract routes from.</param>
    /// <returns></returns>
    private List<string> ExtractRoutes(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var routes = new List<string>();

        // Get Route attribute on class
        var classRouteAttribute = classDeclarationSyntax.AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(attr => attr.Name.ToString().Contains("Route"));

        string baseRoute = string.Empty;
        if(classRouteAttribute != null && classRouteAttribute.ArgumentList?.Arguments.Count > 0)
        {
            baseRoute = classRouteAttribute.ArgumentList.Arguments[0].ToString().Trim('"');
        }

        // Get routes from methods
        var methods = classDeclarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
        foreach(var method in methods)
        {
            var httpAttributes = method.AttributeLists
                .SelectMany(als => als.Attributes)
                .Where(attrSyntax => attrSyntax.Name.ToString().Contains("Http") ||
                                                    attrSyntax.Name.ToString().Contains("Get") ||
                                                    attrSyntax.Name.ToString().Contains("Post"));

            foreach(var attrSyntax in httpAttributes)
            {
                string methodRoute = string.Empty;
                if(attrSyntax.ArgumentList?.Arguments.Count > 0)
                {
                    methodRoute = attrSyntax.ArgumentList.Arguments[0].ToString().Trim('"');
                }

                var fullRoute = string.IsNullOrEmpty(baseRoute)
                    ? methodRoute
                    : $"{baseRoute}/{methodRoute}".TrimEnd('/');

                if (!string.IsNullOrWhiteSpace(fullRoute))
                {
                    routes.Add(fullRoute);
                }
            }
        }

        return routes;
    }

    /// <summary>
    /// Determines whether a type symbol inherits from a specified base type by traversing the inheritance chain. 🦢
    /// </summary>
    /// <param name="symbol">The named type symbol to check for inheritance.</param>
    /// <param name="baseTypeName">The name of the base type to search for in the inheritance chain.</param>
    /// <returns>
    /// <c>true</c> if the symbol's base type matches the specified base type name at any level in the inheritance hierarchy;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method performs a traversal of the inheritance chain starting from the immediate base type.
    /// It uses simple name comparison, so it will match the base type name regardless of its namespace.
    /// If the symbol has no base type or the base type cannot be determined, the method returns <c>false</c>.
    /// </remarks>
    private bool InheritsFrom(INamedTypeSymbol symbol, string baseTypeName)
    {
        var current = symbol.BaseType;
        while(current != null)
        {
            if (current.Name == baseTypeName)
                return true;
            current = current.BaseType;
        }
        return false;
    }
}
