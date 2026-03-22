using Clarity.Dev.NET.Core.Models.Contracts;

namespace Clarity.Dev.NET.Analyzer.Core;

public class CommandParser : ICommandParser
{
    public IAnalysisCommand Parse(string[] args, IConfigurationRoot config)
    {
        IAnalysisCommand command = new AnalysisCommand();

        if (args.Length == 0)
        {
            ApplyDefaults(command, config);
            return command;
        }

        // Pre-scan for help and version flags
        for (int i = 0; i < args.Length; i++)
        {
            if (SolutionCommandConstantsHelper.IsHelpOption(args[i]))
            {
                command.IsHelp = true;
                return command;
            }

            if (SolutionCommandConstantsHelper.IsVersionOption(args[i]))
            {
                command.IsVersion = true;
                return command;
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (i == 0 &&
                !SolutionCommandConstantsHelper.IsOutputOption(args[i]) &&
                !SolutionCommandConstantsHelper.IsOutputPathOption(args[i]) &&
                !SolutionCommandConstantsHelper.IsPathOption(args[i]))
            {
                command.SolutionPath = args[0];
            }
            else if (SolutionCommandConstantsHelper.IsOutputOption(args[i]) && (i + 1) < args.Length)
            {
                if (!OutputFormatTypesHelper.IsValidOutputFormat(args[i + 1]))
                {
                    throw new Exception("Invalid Output Format!");
                }
                if (File.Exists(args[i + 1]))
                {
                    throw new Exception("Output file already exists!");
                }
                if (!string.IsNullOrEmpty(command.OutputFormat))
                {
                    // The output format has already been set, skip to avoid overwriting
                    continue;
                }
                command.OutputFormat = args[i + 1];
                i++;
            }
            else if (SolutionCommandConstantsHelper.IsOutputPathOption(args[i]) && (i + 1) < args.Length)
            {
                if (!string.IsNullOrEmpty(command.OutputPath))
                {
                    // The output path has already been set, skip to avoid overwriting
                    continue;
                }
                command.OutputPath = args[i + 1];
                i++;
            }
            else if (SolutionCommandConstantsHelper.IsPathOption(args[i]) && (i + 1) < args.Length)
            {
                if (!string.IsNullOrEmpty(command.SolutionPath))
                {
                    // The output path has already been set, skip to avoid overwriting
                    continue;
                }
                command.SolutionPath = args[i + 1];
                i++;
            }
            else
            {
                throw new Exception($"Unrecognized argument: {args[i]}");
            }
        }

        ApplyDefaults(command, config);

        return command;
    }

    private void ApplyDefaults(IAnalysisCommand command, IConfigurationRoot config)
    {
        // If the solution path is not provided, we can attempt to find a .sln file in the current directory
        if (string.IsNullOrEmpty(command.SolutionPath))
        {
            // If we're on debug, we can look for the default solution path in the appsettings.json file, otherwise set the current directory as the solution path
            #if DEBUG
                command.SolutionPath = config.GetSection("DefaultTestProject").Value ?? GetDefaultSolutionPath();
            #else
                command.SolutionPath = GetDefaultSolutionPath();   
            #endif
        }
        if (string.IsNullOrEmpty(command.OutputPath))
        {
            command.OutputPath = GetDefaultOutputPath();
        }
        if (string.IsNullOrEmpty(command.OutputFormat))
        {
            command.OutputFormat = GetDefaultOutputFormat();
        }
    }

    private string GetDefaultSolutionPath() => Directory.GetCurrentDirectory();

    private string GetDefaultOutputPath() => Path.Combine(Directory.GetCurrentDirectory(), "Clarity.Dev.Output");

    private string GetDefaultOutputFormat() => OutputFormatTypes.html.ToString();
}
