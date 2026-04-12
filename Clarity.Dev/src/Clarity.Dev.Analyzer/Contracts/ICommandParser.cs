namespace Clarity.Dev.NET.Analyzer.Contracts;

public interface ICommandParser
{
    IAnalysisCommand Parse(string[] args, IConfigurationRoot config);
}
