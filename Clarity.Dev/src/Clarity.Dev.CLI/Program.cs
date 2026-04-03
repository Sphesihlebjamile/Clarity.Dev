using Microsoft.Extensions.DependencyInjection;
using Clarity.Dev.CLI.Contracts;
using Clarity.Dev.CLI.Services;
using Clarity.Dev.Reports.Contracts;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfigurationRoot>(config)
    .AddSingleton<IConfiguration>(config)
    .AddTransient<IConsoleService, ConsoleService>()
    .AddTransient<ICommandParser, CommandParser>()
    .AddTransient<IVersionProvider, VersionProvider>()
    .AddTransient<IApplicationService, ApplicationService>()
    
    .AddTransient<IProjectParser, ProjectParser>()
    .AddTransient<IServiceDetector, ServiceDetector>()
    .AddTransient<ISlnxParser, SlnxParser>()
    .AddTransient<ICommunicationAnalyzer, CommunicationAnalyzer>()
    .AddTransient<ICircularDependencyDetector, CircularDependencyDetector>()
    .AddTransient<ISolutionScanner, SolutionScanner>()

    .AddSingleton<IHtmlReportGenerator, HtmlReportGenerator>()
    .BuildServiceProvider();

var app = serviceProvider.GetRequiredService<IApplicationService>();
return await app.RunAsync(args);