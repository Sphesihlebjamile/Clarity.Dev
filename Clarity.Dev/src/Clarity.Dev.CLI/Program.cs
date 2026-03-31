
try
{
    IConsoleService consoleService = new ConsoleService();
    var cliVersion = GetCliVersion();

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    ICommandParser commandParser = new CommandParser();
    var command = commandParser.Parse(args, config);

    if (command.IsVersion)
    {
        Console.WriteLine($"Clarity.Dev CLI Version: {cliVersion}");
        return;
    }

    if (command.IsHelp)
    {
        consoleService.DisplayHeader(cliVersion);
        consoleService.DisplayHelp();
    }

    await SolutionAnalyzer.AnalyzeSolution(command, consoleService);
}
catch (Exception e)
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