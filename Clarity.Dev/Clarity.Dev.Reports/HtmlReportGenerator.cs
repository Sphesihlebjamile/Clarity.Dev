

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
        htmlBuilder.AppendLine("        <h1>Hello World!</h1>");
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
        throw new NotImplementedException();
    }

    private string GenerateServiceCommunicationDiagram(SolutionModels.SolutionAnalysisResult result)
    {
        throw new NotImplementedException();
    }

    private string GenerateProjectSection(SolutionModels.ProjectInfo project)
    {
        throw new NotImplementedException();
    }

    private string GetEmbeddedStyles()
    {
        return @"
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f7fa; color: #333; line-height: 1.6; }
            .container { max-width: 1200px; margin: 0 auto; padding: 20px; }
            h1 { font-size: 2.5em; margin-bottom: 10px; color: #2c3e50; }
            h2 { font-size: 1.8em; margin-bottom: 15px; color: #34495e; }
            h3 { font-size: 1.3em; color: #2c3e50; }
            .metadata { color: #7f8c8d; margin-bottom: 30px; }
            .summary-card, .warning-card, .diagram-card, .projects-section { background: white; padding: 25px; margin-bottom: 25px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
            .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 20px; margin-top: 15px; }
            .stat { text-align: center; padding: 15px; background: #ecf0f1; border-radius: 5px; }
            .stat strong { font-size: 2em; color: #3498db; display: block; margin-bottom: 5px; }
            .warning-card { border-left: 4px solid #e74c3c; background: #ffe8e6; }
            .warning-card h2 { color: #c0392b; }
            .project-card { margin-bottom: 15px; border: 1px solid #ddd; border-radius: 5px; padding: 15px; background: #fafafa; }
            .project-card summary { cursor: pointer; font-weight: bold; }
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
