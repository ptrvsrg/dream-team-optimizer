using System.CommandLine;
using DreamTeamOptimizer.ConsoleApp.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DreamTeamOptimizer.ConsoleApp.CLI;

public class AverageHarmonicCommand
{
    public static Command New()
    {
        var command = new Command("average-harmonic", "Average Harmonic by all hackathons");
        command.SetHandler(Handle);

        return command;
    }

    private static Task Handle()
    {
        
        return Host
            .CreateApplicationBuilder()
            .ConfigureAll<AverageHarmonicWorker>()
            .Build()
            .MigrateDatabases()
            .RunAsync();
    }
}