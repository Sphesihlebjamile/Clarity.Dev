namespace Clarity.Dev.NET.Core.Models.Enums;

public enum OutputFormatTypes
{
    html,
    json,
    both
}

public static class OutputFormatTypesHelper
{
    public static bool TryParse(string value, out OutputFormatTypes result)
    {
        return Enum.TryParse<OutputFormatTypes>(value, ignoreCase: true, out result);
    }

    public static bool IsValidOutputFormat(string value)
    {
        return TryParse(value, out _);
    }
}
