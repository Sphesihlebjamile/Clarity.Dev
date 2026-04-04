using Microsoft.Extensions.DependencyInjection;
using Clarity.Dev.CLI.Contracts;
using Clarity.Dev.CLI.Services;
using Clarity.Dev.Reports.Contracts;

namespace Clarity.Dev.CLI.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Clarity.Dev CLI services into the DI container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Optional configuration root. If not provided, one will be built from appsettings.json.</param>
    public static IServiceCollection AddClarityCli(
        this IServiceCollection services,
        IConfigurationRoot? configuration = null)
    {
        // Build configuration if not externally provided
        var config = configuration ?? new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configuration — Singleton: shared, immutable for the lifetime of the app
        services.AddSingleton<IConfigurationRoot>(config);
        services.AddSingleton<IConfiguration>(config);

        // CLI services — Singleton: stateless, safe to reuse across operations
        services.AddSingleton<IConsoleService, ConsoleService>();
        services.AddSingleton<ICommandParser, CommandParser>();
        services.AddSingleton<IVersionProvider, VersionProvider>();

        // Report generator — Singleton: stateless, produces output from data
        services.AddSingleton<IHtmlReportGenerator, HtmlReportGenerator>();

        // Analyzer components — Transient: each scan is a fresh operation
        services.AddTransient<IProjectParser, ProjectParser>();
        services.AddTransient<IServiceDetector, ServiceDetector>();
        services.AddTransient<ISlnxParser, SlnxParser>();
        services.AddTransient<ICommunicationAnalyzer, CommunicationAnalyzer>();
        services.AddTransient<ICircularDependencyDetector, CircularDependencyDetector>();
        services.AddTransient<ISolutionScanner, SolutionScanner>();
        services.AddTransient<ISolutionAnalyzer, SolutionAnalyzer>();

        // Application entry point — Transient: resolves fresh per invocation
        services.AddTransient<IApplicationService, ApplicationService>();

        return services;
    }
}
