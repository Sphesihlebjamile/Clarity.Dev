

namespace Clarity.Dev.Reports;

public class HtmlReportGenerator
{
    public string GenerateReport(SolutionModels.SolutionAnalysisResult result, string outputPath)
    {
        StringBuilder htmlBuilder = new();

        // HTML header with embedded styles and Mermaid.js
        htmlBuilder.AppendLine("<!DOCTYPE html>");
        htmlBuilder.AppendLine("<html lang='en'>");
        htmlBuilder.AppendLine("<head>");
        htmlBuilder.AppendLine("    <meta charset='UTF-8'>");
        htmlBuilder.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        htmlBuilder.AppendLine($"    <title>{result.SolutionName} - Analysis Report</title>");
        htmlBuilder.AppendLine("    <script src='https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.min.js'></script>");
        htmlBuilder.AppendLine("    <style>");
        htmlBuilder.AppendLine(GetEmbeddedStyles());
        htmlBuilder.AppendLine("    </style>");
        htmlBuilder.AppendLine("</head>");

        htmlBuilder.AppendLine("<body>");

        // Content container
        htmlBuilder.AppendLine("    <div class='container'>");
        htmlBuilder.AppendLine($"        <h1>{result.SolutionName}</h1>");
        htmlBuilder.AppendLine($"        <p class='metadata'>Analysis Report | Generated on {result.AnalyzedAt:yyyy-MM-dd HH:mm:ss} UTC</p>");

        // Summary Statistics
        htmlBuilder.AppendLine("        <div class='summary-card'>");
        htmlBuilder.AppendLine("            <h2>Summary Statistics</h2>");
        htmlBuilder.AppendLine("            <div class='stats-grid'>");
        htmlBuilder.AppendLine($"                <div class='stat'><strong>{result.Statistics.TotalProjects}</strong><br/>Projects</div>");
        htmlBuilder.AppendLine($"                <div class='stat'><strong>{result.Statistics.TotalNuGetPackages}</strong><br/>NuGet Packages</div>");
        htmlBuilder.AppendLine($"                <div class='stat'><strong>{result.Statistics.TotalServices}</strong><br/>Services</div>");
        htmlBuilder.AppendLine($"                <div class='stat'><strong>{result.Statistics.AnalysisDuration.TotalSeconds}</strong><br/>Analysis Time (s)</div>");
        htmlBuilder.AppendLine("            </div>");
        htmlBuilder.AppendLine("        </div>");

        // Circular Dependency Warnings (if any)
        if (result.CircularDependencies.Any())
        {
            htmlBuilder.AppendLine("        <div class='warning-card'>");
            htmlBuilder.AppendLine($"            <h2>Circular Dependencies ({result.CircularDependencies.Count})</h2>");
            htmlBuilder.AppendLine("            <ul>");
            foreach(var cycle in result.CircularDependencies)
            {
                htmlBuilder.AppendLine($"                <li>{cycle.Description}</li>");
            }
            htmlBuilder.AppendLine("            </ul>");
            htmlBuilder.AppendLine("        </div>");
        }

        // Project Dependency Graph
        htmlBuilder.AppendLine("        <div class='diagram-card'>");
        htmlBuilder.AppendLine("            <h2>Project Dependency Graph</h2>");
        htmlBuilder.AppendLine("            <div class='mermaid'>");
        htmlBuilder.AppendLine(GenerateProjectDependencyDiagram(result.Projects));
        htmlBuilder.AppendLine("            </div>");
        htmlBuilder.AppendLine("        </div>");

        // Service communication diagram
        if (result.ServiceCommunications.Any())
        {
            htmlBuilder.AppendLine("        <div class='diagram-card'>");
            htmlBuilder.AppendLine("            <h2>Service Communication Map</h2>");
            htmlBuilder.AppendLine("            <div class='mermaid'>");
            htmlBuilder.AppendLine(GenerateServiceCommunicationDiagram(result));
            htmlBuilder.AppendLine("            </div>");
            htmlBuilder.AppendLine("        </div>");
        }

        // Project details
        htmlBuilder.AppendLine("        <div class='projects-section'>");
        htmlBuilder.AppendLine("            <h2>Project Details</h2>");
        foreach(var project in result.Projects.OrderBy(proj => proj.Name))
        {
            htmlBuilder.AppendLine(GenerateProjectSection(project));
        }
        htmlBuilder.AppendLine("        </div>");

        // Container ending
        htmlBuilder.AppendLine("    </div>");

        // Initialize Mermaid
        htmlBuilder.AppendLine("    <script>");
        htmlBuilder.AppendLine("        mermaid.initialize({ startOnLoad: true, theme: 'default' });");
        htmlBuilder.AppendLine("    </script>");

        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");

        var fullPath = Path.GetFullPath(outputPath);
        File.WriteAllText(fullPath, htmlBuilder.ToString());
        return fullPath;
    }

    private string GenerateProjectDependencyDiagram(List<SolutionModels.ProjectInfo> projects)
    {
        var htmlBuilder = new StringBuilder();

        // Add Mermaid configuration for better layout
        htmlBuilder.AppendLine("%%{init: {");
        htmlBuilder.AppendLine("  'flowchart': {");
        htmlBuilder.AppendLine("    'curve': 'linear',");
        htmlBuilder.AppendLine("    'rankSpacing': 80,");
        htmlBuilder.AppendLine("    'nodeSpacing': 50,");
        htmlBuilder.AppendLine("    'htmlLabels': true");
        htmlBuilder.AppendLine("  },");
        htmlBuilder.AppendLine("  'theme': 'default'");
        htmlBuilder.AppendLine("}}%%");

        htmlBuilder.AppendLine("graph LR");

        // Categorize projects by type for better visual organization
        var testProjects = projects.Where(p => p.IsTestProject).ToList();
        var domainProjects = projects.Where(p => !p.IsTestProject && p.Name.Contains("Domain")).ToList();
        var infrastructureProjects = projects.Where(p => !p.IsTestProject && p.Name.Contains("Infrastructure")).ToList();
        var applicationProjects = projects.Where(p => !p.IsTestProject && !p.Name.Contains("Domain") && !p.Name.Contains("Infrastructure")).ToList();

        // Add nodes with styling based on type
        foreach (var project in domainProjects)
        {
            var projectId = SanitizeForMermaid(project.Name);
            htmlBuilder.AppendLine($"    {projectId}[\"{project.Name}\"]");
            htmlBuilder.AppendLine($"    style {projectId} fill:#e1f5ff,stroke:#01579b,stroke-width:2px");
        }

        foreach (var project in infrastructureProjects)
        {
            var projectId = SanitizeForMermaid(project.Name);
            htmlBuilder.AppendLine($"    {projectId}[\"{project.Name}\"]");
            htmlBuilder.AppendLine($"    style {projectId} fill:#f3e5f5,stroke:#4a148c,stroke-width:2px");
        }

        foreach (var project in applicationProjects)
        {
            var projectId = SanitizeForMermaid(project.Name);
            htmlBuilder.AppendLine($"    {projectId}[\"{project.Name}\"]");
            htmlBuilder.AppendLine($"    style {projectId} fill:#e8f5e9,stroke:#1b5e20,stroke-width:2px");
        }

        foreach (var project in testProjects)
        {
            var projectId = SanitizeForMermaid(project.Name);
            htmlBuilder.AppendLine($"    {projectId}[\"{project.Name}\"]");
            htmlBuilder.AppendLine($"    style {projectId} fill:#fff3e0,stroke:#e65100,stroke-width:2px,stroke-dasharray: 5 5");
        }

        // Add all dependencies
        foreach (var project in projects)
        {
            var projectId = SanitizeForMermaid(project.Name);

            foreach (var dependency in project.ProjectReferences)
            {
                var depId = SanitizeForMermaid(dependency);
                htmlBuilder.AppendLine($"    {projectId} --> {depId}");
            }
        }

        return htmlBuilder.ToString();
    }

    private string GenerateServiceCommunicationDiagram(SolutionModels.SolutionAnalysisResult result)
    {
        var htmlBuilder = new StringBuilder();

        // Mermaid init for straighter layout and consistent styling
        htmlBuilder.AppendLine("%%{init:{");
        htmlBuilder.AppendLine("  'flowchart': { 'curve': 'linear', 'rankSpacing': 60, 'nodeSpacing': 50, 'htmlLabels': true },");
        htmlBuilder.AppendLine("  'theme': 'default'");
        htmlBuilder.AppendLine("}}%%");
        htmlBuilder.AppendLine("graph LR");

        // Aggregate communications by source, target and type so we don't draw many parallel edges
        var aggregated = result.ServiceCommunications
            .GroupBy(c => new { c.SourceProject, c.TargetService, c.Type })
            .Select(g => new { g.Key.SourceProject, g.Key.TargetService, g.Key.Type, Count = g.Count() })
            .ToList();

        var declared = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        string DeclareNode(string id, string label, string kind)
        {
            if (!declared.Contains(id))
            {
                htmlBuilder.AppendLine($"    {id}[\"{label}\"]");
                // simple styles: services (targets) purple, applications green, tests orange
                if (kind == "service")
                    htmlBuilder.AppendLine($"    style {id} fill:#f3e5f5,stroke:#4a148c,stroke-width:2px");
                else if (kind == "test")
                    htmlBuilder.AppendLine($"    style {id} fill:#fff3e0,stroke:#e65100,stroke-width:2px,stroke-dasharray: 5 5");
                else
                    htmlBuilder.AppendLine($"    style {id} fill:#e8f5e9,stroke:#1b5e20,stroke-width:2px");

                declared.Add(id);
            }
            return id;
        }

        foreach (var entry in aggregated)
        {
            var sourceId = SanitizeForMermaid(entry.SourceProject);
            var targetId = SanitizeForMermaid(entry.TargetService);
            var typeLabel = GetCommunicationTypeLabel(entry.Type);
            var labelWithCount = entry.Count > 1 ? $"{typeLabel} ({entry.Count})" : typeLabel;

            // Heuristics to choose node kind for styling
            var sourceKind = (entry.SourceProject?.IndexOf("test", StringComparison.OrdinalIgnoreCase) >= 0 || entry.SourceProject?.EndsWith("Tests") == true)
                ? "test" : "app";
            var targetKind = "service";

            DeclareNode(sourceId, entry.SourceProject, sourceKind);
            DeclareNode(targetId, entry.TargetService, targetKind);

            htmlBuilder.AppendLine($"    {sourceId} -->|\"{labelWithCount}\"| {targetId}");
        }

        return htmlBuilder.ToString();
    }

    private string GetCommunicationTypeLabel(SolutionModels.Enums.CommunicationType type)
    {
        return type switch
        {
            SolutionModels.Enums.CommunicationType.HttpClient => "HTTP Client",
            SolutionModels.Enums.CommunicationType.GrpcClient => "gRPC Client",
            SolutionModels.Enums.CommunicationType.MessageQueue => "Message Queue",
            SolutionModels.Enums.CommunicationType.Database => "Database",
            SolutionModels.Enums.CommunicationType.SignalR => "SignalR",
            _ => "Unknown"
        };
    }

    private string GenerateProjectSection(SolutionModels.ProjectInfo project)
    {
        var sb = new StringBuilder();
        var isTest = project.IsTestProject ? " 🧪" : "";

        sb.AppendLine("            <details class='project-card'>");
        sb.AppendLine($"                <summary><h3>{project.Name}{isTest}</h3></summary>");
        sb.AppendLine("                <div class='project-details'>");
        sb.AppendLine($"                    <p><strong>Target Framework:</strong> {project.TargetFramework}</p>");
        sb.AppendLine($"                    <p><strong>C# Version:</strong> {project.CSharpVersion}</p>");
        sb.AppendLine($"                    <p><strong>Output Type:</strong> {project.OutputType}</p>");

        // NuGet packages
        if (project.NuGetDependencies.Any())
        {
            sb.AppendLine("                    <h4>📦 NuGet Dependencies</h4>");
            sb.AppendLine("                    <table>");
            sb.AppendLine("                        <thead><tr><th>Package</th><th>Version</th></tr></thead>");
            sb.AppendLine("                        <tbody>");
            foreach (var dep in project.NuGetDependencies.OrderBy(d => d.PackageName))
            {
                sb.AppendLine($"                            <tr><td>{dep.PackageName}</td><td>{dep.Version}</td></tr>");
            }
            sb.AppendLine("                        </tbody>");
            sb.AppendLine("                    </table>");
        }

        // Detected services
        if (project.DetectedServices.Any())
        {
            sb.AppendLine("                    <h4>🔧 Detected Services</h4>");
            sb.AppendLine("                    <ul>");
            foreach (var service in project.DetectedServices)
            {
                //var icon = GetServiceIcon(service.Type);
                sb.AppendLine($"                        <li><strong>{service.Name}</strong> ({service.Type})");
                if (service.Routes.Any())
                {
                    sb.AppendLine("                            <ul>");
                    foreach (var route in service.Routes)
                    {
                        sb.AppendLine($"                                <li><code>{route}</code></li>");
                    }
                    sb.AppendLine("                            </ul>");
                }
                sb.AppendLine("                        </li>");
            }
            sb.AppendLine("                    </ul>");
        }

        sb.AppendLine("                </div>");
        sb.AppendLine("            </details>");

        return sb.ToString();
    }

    private string SanitizeForMermaid(string input)
    {
        // Remove special characters that might break Mermaid syntax and replace spaces with underscores
        return input
            .Replace(".", "_")
            .Replace("-", "_")
            .Replace(" ", "_")
            .Replace("(", "_")
            .Replace(")", "_")
            .Replace("[", "_")
            .Replace("]", "_")
            .Replace("{", "_")
            .Replace("}", "_")
            .Replace("|", "_")
            .Replace("\"", "_")
            .Replace("'", "_");
    }

    private string GetEmbeddedStyles()
    {
        return @"
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f7fa; color: #333; line-height: 1.6; }
            .container { max-width: 1200px; margin: 0 auto; padding: 20px; }
            h1 { font-size: 2.5em; margin-bottom: 10px; color: #2c3e50; }
            h2 { font-size: 1.8em; margin-bottom: 15px; color: #34495e; }
            h3 { font-size: 1.3em; color: #2c3e50; white-space: nowrap; }
            .metadata { color: #7f8c8d; margin-bottom: 30px; }
            .summary-card, .warning-card, .diagram-card, .projects-section { background: white; padding: 25px; margin-bottom: 25px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
            .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 20px; margin-top: 15px; }
            .stat { text-align: center; padding: 15px; background: #ecf0f1; border-radius: 5px; }
            .stat strong { font-size: 2em; color: #3498db; display: block; margin-bottom: 5px; }
            .warning-card { border-left: 4px solid #e74c3c; background: #ffe8e6; }
            .warning-card h2 { color: #c0392b; }
            .project-card { margin-bottom: 15px; border: 1px solid #ddd; border-radius: 5px; padding: 15px; background: #fafafa; }
            .project-card summary { cursor: pointer; font-weight: bold; display: flex; align-items: center; white-space: nowrap; }
            .project-card summary h3 { margin: 0; white-space: nowrap; }
            .project-card summary:hover { color: #3498db; }
            .project-details { margin-top: 15px; padding-left: 20px; }
            table { width: 100%; border-collapse: collapse; margin-top: 10px; }
            th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }
            th { background: #34495e; color: white; }
            tr:hover { background: #f5f5f5; }
            code { background: #ecf0f1; padding: 2px 6px; border-radius: 3px; font-family: 'Courier New', monospace; }
            ul { margin-left: 20px; }
            li { margin-bottom: 5px; }
        ";
    }
}
