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
        var builder = Host.CreateApplicationBuilder();
        
        builder.ConfigureConfiguration();
        builder.ConfigureLogger();
        builder.ConfigureStrategy();
        builder.ConfigureRepositories();
        builder.ConfigureServices();
        
        builder.Services.AddHostedService<HackathonConductWorker>();
        
        return builder.Build().MigrateDatabases().RunAsync();
    }
}