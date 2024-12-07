using System.CommandLine;
using DreamTeamOptimizer.ConsoleApp.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DreamTeamOptimizer.ConsoleApp.CLI;

public class ConductCommand
{
    public static Command New()
    {
        var command = new Command("conduct", "Conduct a hackathon");
        command.SetHandler(Handle);

        return command;
    }

    private static Task Handle()
    {
        return Host
            .CreateApplicationBuilder()
            .ConfigureAll<HackathonConductWorker>()
            .Build()
            .MigrateDatabases()
            .RunAsync();
    }
}