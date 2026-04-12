namespace Clarity.Dev.CLI.Services;

public class VersionProvider : IVersionProvider
{
    private readonly IConfiguration _configuration;

    public VersionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetCliVersion()
    {
        // 1. Try appsettings.json
        var configVersion = _configuration.GetSection("AppSettings:Version").Value;
        if (!string.IsNullOrWhiteSpace(configVersion))
            return configVersion;

        // 2. Try assembly informational version (set via <AssemblyInformationalVersion> in .csproj)
        // The .NET SDK appends git commit SHA as build metadata (e.g. "1.0.0-beta+abc123"), so we strip it.
        var assemblyVersion = Assembly
            .GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')[0];
        if (!string.IsNullOrWhiteSpace(assemblyVersion))
            return assemblyVersion;

        // 3. Hard-coded default
        return "1.0.0";
    }
}
