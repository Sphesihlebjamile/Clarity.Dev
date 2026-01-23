

Console.WriteLine("╔════════════════════════════════════════╗");
Console.WriteLine("║   Clarity.Dev: Solution Analyzer       ║");
Console.WriteLine("╚════════════════════════════════════════╝");
Console.WriteLine();

#if DEBUG
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
    args = [config["DefaultTestProject"] ?? string.Empty];
#endif

await SolutionAnalyzer.AnalyzeSolution(args[0]);


