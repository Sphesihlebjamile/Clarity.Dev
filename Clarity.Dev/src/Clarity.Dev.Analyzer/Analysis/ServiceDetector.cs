namespace Clarity.Dev.NET.Analyzer.Analysis;

/// <summary>
/// Detects various types of services in a .NET project 🦢
/// </summary>
public class ServiceDetector: IServiceDetector
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

    /// <summary>
    /// Identifies controller classes within the specified syntax tree and returns information about each detected
    /// controller 🦢
    /// </summary>
    /// <remarks>
    /// A class is considered a controller if it inherits from ControllerBase or Controller, or is
    /// decorated with the ApiController attribute.
    /// </remarks>
    /// <param name="root">The root syntax node of the C# source file to analyze for controller classes.</param>
    /// <param name="semanticModel">The semantic model used to provide symbol information for the syntax tree.</param>
    /// <param name="filePath">The file path of the source file being analyzed. Used to associate detected controllers with their source file.</param>
    /// <returns>
    /// A list of ServiceInfo objects representing each controller class found in the syntax tree. The list is empty if
    /// no controllers are detected.
    /// </returns>
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

    /// <summary>
    /// Identifies classes in the specified syntax tree that represent background services by inheriting from
    /// BackgroundService or implementing IHostedService.
    /// </summary>
    /// <remarks>
    /// This method is typically used to analyze C# source files for ASP.NET Core background
    /// services, such as those based on BackgroundService or IHostedService. Only top-level class declarations are
    /// considered.
    /// </remarks>
    /// <param name="root">The root syntax node of the C# syntax tree to analyze.</param>
    /// <param name="semanticModel">The semantic model used to resolve type information for the syntax tree.</param>
    /// <param name="filePath">
    /// The file path associated with the syntax tree being analyzed. Used to record the source location of detected services.
    /// </param>
    /// <returns>
    /// A list of ServiceInfo objects describing each detected background service. The list is empty if no matching
    /// services are found.
    /// </returns>
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

    /// <summary>
    /// Identifies all classes in the specified syntax tree that inherit from SignalR's Hub class 🦢
    /// </summary>
    /// <remarks>
    /// This method examines all class declarations in the provided syntax tree and determines
    /// whether each class inherits from the SignalR Hub base class. Only direct or indirect subclasses of Hub are
    /// included in the result.
    /// </remarks>
    /// <param name="root">The root syntax node of the C# syntax tree to analyze.</param>
    /// <param name="semanticModel">The semantic model used to resolve type information for the syntax tree.</param>
    /// <param name="filePath">The file path associated with the syntax tree being analyzed. Used to populate the returned service information.</param>
    /// <returns>
    /// A list of ServiceInfo objects representing SignalR hub classes found in the syntax tree. The list is empty if no
    /// such classes are detected.
    /// </returns>
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

    /// <summary>
    /// Identifies gRPC service classes within the specified syntax tree 🦢
    /// </summary>
    /// <remarks>
    /// A class is considered a gRPC service if it inherits from a base class whose name ends with
    /// "Base" and whose namespace contains "Grpc". This method does not validate service implementation details beyond
    /// these inheritance patterns.
    /// </remarks>
    /// <param name="root">The root syntax node of the C# source file to analyze.</param>
    /// <param name="semanticModel">The semantic model used to resolve type information for the syntax tree.</param>
    /// <param name="filePath">The file path of the source file being analyzed. Used to associate detected services with their source location.</param>
    /// <returns>
    /// A list of ServiceInfo objects representing the gRPC service classes found in the syntax tree. The list is empty
    /// if no gRPC services are detected.
    /// </returns>
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

    /// <summary>
    /// Detects Minimal API endpoint definitions within the specified syntax tree 🦢
    /// </summary>
    /// <remarks>
    /// This method identifies Minimal API endpoints by searching for method invocations such as
    /// app.MapGet, app.MapPost, app.MapPut, app.MapDelete, and app.MapPatch. Each returned <see cref="ServiceInfo"/>
    /// groups endpoints by file to avoid duplication.
    /// </remarks>
    /// <param name="root">The root <see cref="SyntaxNode"/> of the syntax tree to analyze for Minimal API endpoints.</param>
    /// <param name="filePath">The file path associated with the syntax tree. Used to group detected endpoints by file.</param>
    /// <returns>
    /// A list of <see cref="ServiceInfo"/> objects representing Minimal API endpoints found in the syntax tree. The
    /// list is empty if no endpoints are detected.
    /// </returns>
    private List<ServiceInfo> DetectMinimalApis(SyntaxNode root, string filePath)
    {
        var apis = new List<ServiceInfo>();

        // Look for app.MapGet, app.MapPost, etc.
        var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocations)
        {
            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) continue;

            var methodName = memberAccess.Name.Identifier.ValueText;
            if (methodName.StartsWith("Map") && (methodName.Contains("Get") || methodName.Contains("Post") ||
                methodName.Contains("Put") || methodName.Contains("Delete") || methodName.Contains("Patch")))
            {
                // Extract route if available
                var route = invocation.ArgumentList.Arguments.FirstOrDefault()?.ToString() ?? "";
                route = route.Trim('"');

                // Only add one MinimalApi entry per file to avoid duplication
                if (!apis.Any(a => a.Type == ServiceType.MinimalApi && a.FilePath == filePath))
                {
                    apis.Add(new ServiceInfo
                    {
                        Name = "Minimal API Endpoints",
                        Type = ServiceType.MinimalApi,
                        FilePath = filePath,
                        Routes = new List<string> { route }
                    });
                }
                else
                {
                    // Add route to existing entry
                    apis.First(a => a.Type == ServiceType.MinimalApi && a.FilePath == filePath)
                        .Routes.Add(route);
                }
            }
        }

        return apis;
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
