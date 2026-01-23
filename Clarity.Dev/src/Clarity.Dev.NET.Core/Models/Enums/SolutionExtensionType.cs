namespace Clarity.Dev.NET.Core.Models.Enums;

public enum SolutionExtensionType
{
    sln,
    slnx
}

public static class SolutionExtensionTypeHelper
{
    public static bool IsSlnFile(this string extension) => 
        extension == GetExtension(SolutionExtensionType.sln);
    
    public static bool IsSlnxFile(this string extension) =>
        extension == GetExtension(SolutionExtensionType.slnx);

    private static string GetExtension(SolutionExtensionType extensionType)
    {
        return extensionType switch
        {
            SolutionExtensionType.sln => ".sln",
            SolutionExtensionType.slnx => ".slnx",
            _ => throw new ArgumentOutOfRangeException(nameof(extensionType), extensionType, null)
        };
    }
}