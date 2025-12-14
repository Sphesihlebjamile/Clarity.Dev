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
        var controllers = new List<ServiceInfo>();

        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (symbol == null) continue;

            // Check if inherits from ControllerBase or Controller
            var isController = InheritsFrom((INamedTypeSymbol)symbol, "ControllerBase") 
                || InheritsFrom((INamedTypeSymbol)symbol, "Controller");

            // Or has ApiController attribute
            var hasApiControllerAttr = symbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.Name == "ApiControllerAttribute");

            if (isController || hasApiControllerAttr)
            {
                var routes = ExtractRoutes(classDeclaration);

                controllers.Add(new ServiceInfo
                {
                    Name = symbol.Name,
                    Type = ServiceType.Controller,
                    FilePath = filePath,
                    Routes = routes
                });
            }
        }

        return controllers;
    }

    private List<ServiceInfo> DetectBackgroundServices(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        var services = new List<ServiceInfo>();

        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classDeclarations)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (symbol == null) continue;

            if (InheritsFrom((INamedTypeSymbol)symbol, "BackgroundService") 
                || ImplementsInterface((INamedTypeSymbol)symbol, "IHostedService"))
            {
                services.Add(new ServiceInfo
                {
                    Name = symbol.Name,
                    Type = ServiceType.BackgroundService,
                    FilePath = filePath
                });
            }
        }

        return services;
    }

    private List<ServiceInfo> DetectSignalRHubs(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        var hubs = new List<ServiceInfo>();

        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDecleration in classDeclarations)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDecleration);
            if (symbol == null) continue;

            if (InheritsFrom((INamedTypeSymbol)symbol, "Hub"))
            {
                hubs.Add(new ServiceInfo
                {
                    Name = symbol.Name,
                    Type = ServiceType.SignalRHub,
                    FilePath = filePath
                });
            }
        }

        return hubs;
    }

    private List<ServiceInfo> DetectGrpcServices(SyntaxNode root, SemanticModel semanticModel, string filePath)
    {
        var services = new List<ServiceInfo>();

        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDecl in classDeclarations)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDecl);
            if (symbol == null) continue;

            // gRPC services typically inherit from generated base classes ending in "Base"
            if (((INamedTypeSymbol)symbol).BaseType?.Name.EndsWith("Base") == true &&
                ((INamedTypeSymbol)symbol).BaseType?.ContainingNamespace != null &&
                ((INamedTypeSymbol)symbol).BaseType?.ContainingNamespace?.ToString()?.Contains("Grpc") == true)
            {
                services.Add(new ServiceInfo
                {
                    Name = symbol.Name,
                    Type = ServiceType.GrpcService,
                    FilePath = filePath
                });
            }
        }

        return services;
    }

    private List<ServiceInfo> DetectMinimalApis(SyntaxNode root, string filePath)
    {
        throw new NotImplementedException($"Have not implemented the {DetectMinimalApis} function!");
    }

    /// <summary>
    /// Extracts all HTTP routes from a controller classDecleration by combining classDecleration-level and method-level route attributes. 🦢
    /// </summary>
    /// <param name="classDeclarationSyntax">The classDecleration declaration syntax to extract routes from.</param>
    /// <returns></returns>
    private List<string> ExtractRoutes(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var routes = new List<string>();

        // Get Route attribute on classDecleration
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

    /// <summary>
    /// Determines whether a type symbol implements a specified interface by traversing all implemented interfaces 🦢
    /// </summary>
    /// <param name="symbol">The named type symbol to check.</param>
    /// <param name="interfaceName">The name of the interface to search for.</param>
    /// <returns><c>true</c> if the symbol implements the specified interface; otherwise, <c>false</c>.</returns>
    private bool ImplementsInterface(INamedTypeSymbol symbol, string interfaceName)
    {
        return symbol.AllInterfaces.Any(i => i.Name == interfaceName);
    }
}
