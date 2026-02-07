namespace Clarity.Dev.CLI.Constants;

internal enum OutputFormatTypes
{
    html,
    json,
    both
}

internal static class OutputFormatTypesHelper
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
