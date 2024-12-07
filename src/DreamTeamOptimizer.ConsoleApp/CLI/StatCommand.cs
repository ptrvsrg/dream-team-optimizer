using System.CommandLine;
using DreamTeamOptimizer.ConsoleApp.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DreamTeamOptimizer.ConsoleApp.CLI;

public class StatCommand
{
    public static Command New()
    {
        var command = new Command("stat", "Show statistics for hackathon by ID");
        var hackathonIdArg = new Argument<int>("ID", "Hackathon ID");
        command.AddArgument(hackathonIdArg);
        command.SetHandler(Handle, hackathonIdArg);

        return command;
    }

    private static Task Handle(int hackathonId)
    {
        var builder = Host
            .CreateApplicationBuilder()
            .ConfigureAll<HackathonConductWorker>();
        builder.Configuration.AddCommandLine(["--hackathonId", hackathonId.ToString()]);

        return builder
            .Build()
            .MigrateDatabases()
            .RunAsync();
    }
}