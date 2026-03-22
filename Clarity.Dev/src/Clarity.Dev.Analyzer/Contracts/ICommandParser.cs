using Clarity.Dev.NET.Core.Models.Contracts;

namespace Clarity.Dev.NET.Analyzer.Contracts;

public interface ICommandParser
{
    IAnalysisCommand Parse(string[] args, IConfigurationRoot config);
}
