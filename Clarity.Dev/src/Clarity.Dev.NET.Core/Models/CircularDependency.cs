namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Represents a circular dependency between projects 🦢
/// </summary>
public class CircularDependency
{
    public List<string> ProjectPath { get; set; }
    public string Description => string.Join(" → ", ProjectPath) + " → " + ProjectPath[0];
}
