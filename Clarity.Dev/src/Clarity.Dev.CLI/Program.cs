
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
        Console.WriteLine("Usage: dotnet clarity-dev [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --path <path>         Path to solution file or directory");
        Console.WriteLine("  --output <format>     Output format (e.g., html)");
        Console.WriteLine("  --output-path <path>  Output directory path");
        Console.WriteLine("  -help                 Display this help message");
        Console.WriteLine("  --version             Display version information");
        return;
    }

    consoleService.DisplayHeader(cliVersion);

    await SolutionAnalyzer.AnalyzeSolution(command, consoleService);
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