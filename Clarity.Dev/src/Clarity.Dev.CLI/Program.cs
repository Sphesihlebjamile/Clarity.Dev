

try
{
    Console.WriteLine("==========================================");
    Console.WriteLine("|   Clarity.Dev: Solution Analyzer       |");
    Console.WriteLine("==========================================");
    Console.WriteLine();

    var solutionAnalysisInput = SolutionAnalyzer.GetOutputCommands(args);
    await SolutionAnalyzer.AnalyzeSolution(solutionAnalysisInput);
}
catch(Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(e.Message);
    Console.WriteLine("Ending program with error.");
}