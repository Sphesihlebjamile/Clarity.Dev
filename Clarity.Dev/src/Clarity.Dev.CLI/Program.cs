

using Clarity.Dev.NET.Analyzer.Contracts;

try
{
    IConsoleService consoleService = new ConsoleService();
    var cliVersion = GetCliVersion();
    consoleService.DisplayHeader("1.0.0-beta");

    var solutionAnalysisInput = SolutionAnalyzer.GetOutputCommands(args);
    await SolutionAnalyzer.AnalyzeSolution(solutionAnalysisInput);
}
catch(Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(e.Message);
    Console.WriteLine("Ending program with error.");
    Console.ResetColor();
}

static string GetCliVersion()
{
    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
    var appVersion = config.GetSection("AppSettings:Version").Value ?? "1.0.0";
    return appVersion;
}