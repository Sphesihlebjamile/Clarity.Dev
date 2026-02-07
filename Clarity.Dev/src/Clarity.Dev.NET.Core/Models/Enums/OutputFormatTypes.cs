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

    public static bool IsHtmlFormat(string value)
    {
        return TryParse(value, out var result) && result == OutputFormatTypes.html;
    }

    public static bool IsJsonFormat(string value)
    {
        return TryParse(value, out var result) && result == OutputFormatTypes.json;
    }

    public static bool IsBothFormat(string value)
    {
        return TryParse(value, out var result) && result == OutputFormatTypes.both;
    }
}
