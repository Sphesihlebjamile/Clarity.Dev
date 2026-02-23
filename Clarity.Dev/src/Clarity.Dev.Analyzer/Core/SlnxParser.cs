namespace Clarity.Dev.NET.Analyzer.Core;

/// <summary>
/// Parses .slnc (XML-based solution) files 🦢
/// </summary>
public class SlnxParser : ISlnxParser
{
    public List<string> ParseSlnx(string slnxPath)
    {
        // Validate Input
        if (string.IsNullOrWhiteSpace(slnxPath))
        {
            throw new ArgumentException("Solution path cannot be null or empty.", nameof(slnxPath));
        }

        // Check if file exists
        if (!File.Exists(slnxPath))
        {
            throw new FileNotFoundException($"Solution file not found: {slnxPath}", slnxPath);
        }

        var projectPaths = new List<string>();
        var slnxDirectory = Path.GetDirectoryName(slnxPath) ?? string.Empty;

        try
        {
            var xdocument = XDocument.Load(slnxPath);
            var projectElements = xdocument.Descendants("Project");

            foreach (var projectElement in projectElements)
            {
                var pathAttribute = projectElement.Attribute("Path");
                if (pathAttribute != null && !string.IsNullOrWhiteSpace(pathAttribute.Value))
                {
                    var projectPath = pathAttribute.Value;

                    // Make path absolute if it's relative
                    if (!Path.IsPathRooted(projectPath))
                    {
                        projectPath = Path.Combine(slnxDirectory, projectPath);
                    }

                    projectPath = Path.GetFullPath(projectPath);
                    projectPaths.Add(projectPath);
                }
            }
        }
        catch(Exception ex) when (ex is not FileNotFoundException && ex is not ArgumentException)
        {
            throw new InvalidOperationException($"Failed to parse .slnx file: {slnxPath}", ex);
        }

        return projectPaths;
    }
}
