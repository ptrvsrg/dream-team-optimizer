using System.CommandLine;
using DreamTeamOptimizer.ConsoleApp.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand();
        
        rootCommand.Add(ConductCommand.New());
        rootCommand.Add(StatCommand.New());
        rootCommand.Add(AverageHarmonicCommand.New());
        
        await rootCommand.InvokeAsync(args);
    }
}

