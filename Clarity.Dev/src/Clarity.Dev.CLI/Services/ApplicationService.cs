namespace Clarity.Dev.CLI.Services;

public class ApplicationService : IApplicationService
{
    private readonly ICommandParser _commandParser;
    private readonly IConsoleService _consoleService;
    private readonly IVersionProvider _versionProvider;
    private readonly IConfigurationRoot _configuration;
    private readonly ISolutionScanner _solutionScanner;
    private readonly IHtmlReportGenerator _htmlReportGenerator;
    private readonly ISolutionAnalyzer _solutionAnalyzer;

    public ApplicationService(
        ICommandParser commandParser, 
        IConsoleService consoleService, 
        IVersionProvider versionProvider,
        IConfigurationRoot configuration,
        ISolutionScanner solutionScanner,
        IHtmlReportGenerator htmlReportGenerator,
        ISolutionAnalyzer solutionAnalyzer)
    {
        _commandParser = commandParser;
        _consoleService = consoleService;
        _versionProvider = versionProvider;
        _configuration = configuration;
        _solutionScanner = solutionScanner;
        _htmlReportGenerator = htmlReportGenerator;
        _solutionAnalyzer = solutionAnalyzer;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = _commandParser.Parse(args, _configuration);
            var cliVersion = _versionProvider.GetCliVersion();

            if (command.IsVersion)
            {
                Console.WriteLine($"Clarity.Dev CLI Version: {cliVersion}");
                return 0;
            }

            if (command.IsHelp)
            {
                DisplayHeader(cliVersion);
                _consoleService.DisplayHelp();
                return 0;
            }

            DisplayHeader(cliVersion);
            return await _solutionAnalyzer.AnalyzeSolution(command, _consoleService, _solutionScanner, _htmlReportGenerator, cancellationToken);
        }
        catch (Exception e)
        {
            _consoleService.DisplayError(e.Message);
            _consoleService.SetForegroundColor(ConsoleColor.Red);
            _consoleService.DisplayInfo("Ending program with error.");
            _consoleService.ResetColor();
            return 1;
        }
    }

    private void DisplayHeader(string cliVersion)
    {
        _consoleService.DisplayHeader(cliVersion);
    }
}
