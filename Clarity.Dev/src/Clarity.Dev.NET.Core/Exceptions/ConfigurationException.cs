namespace Clarity.Dev.NET.Core.Exceptions;

/// <summary>
/// Thrown when application configuration is missing or invalid (e.g. appsettings.json).
/// Exit code: 5
/// </summary>
public class ConfigurationException : CliException
{
    public override int ExitCode => 5;

    public ConfigurationException(string message) : base(message) { }

    public ConfigurationException(string message, Exception innerException) : base(message, innerException) { }
}
