namespace Clarity.Dev.NET.Core.Models.Enums;

public enum ProjectExtensionType
{
    CSPROJ,
    VBPROJ,
    FSPROJ
}

public static class ProjectExtensionTypeHandler
{
    public static bool IsCsproj(string extension) =>
        extension == GetExtension(ProjectExtensionType.CSPROJ);

    public static bool IsVbproj(string extension) =>
        extension == GetExtension(ProjectExtensionType.VBPROJ);

    public static bool IsFsproj(string extension) =>
        extension == GetExtension(ProjectExtensionType.FSPROJ);

    public static string GetExtension(ProjectExtensionType extensionType)
    {
        return extensionType switch
        {
            ProjectExtensionType.CSPROJ => ".csproj",
            ProjectExtensionType.VBPROJ => ".vsproj",
            ProjectExtensionType.FSPROJ => ".fsproj",
            _ => throw new ArgumentOutOfRangeException(nameof(extensionType), extensionType, null)
        };
    }
}