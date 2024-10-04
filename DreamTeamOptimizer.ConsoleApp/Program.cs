using CommandLine;
using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Helpers;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DreamTeamOptimizer.ConsoleApp.HostedServices;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((_, services) => 
            {
                services.AddLogging(logging => logging.ClearProviders().AddSerilog());
                services.AddHostedService<HackathonWorker>();
                services.AddSingleton<Hackathon>();
                services.AddSingleton<CommandLineOptions>(_ => Parser.Default.ParseArguments<CommandLineOptions>(args).Value);
                services.AddSingleton<IStrategy>(provider =>
                {
                    var options = provider.GetRequiredService<CommandLineOptions>();
                    var strategy = options.StrategyType;
                    return StrategyFactory.NewStrategy(strategy);
                });
                services.AddSingleton<List<Junior>>(provider =>
                {
                    var options = provider.GetRequiredService<CommandLineOptions>();
                    var juniorsFilePath = options.JuniorsFilePath;
                    return CsvLoader.Load<Junior>(juniorsFilePath);
                });
                services.AddSingleton<List<TeamLead>>(provider =>
                {
                    var options = provider.GetRequiredService<CommandLineOptions>();
                    var teamLeadsFilePath = options.TeamLeadsFilePath;
                    return CsvLoader.Load<TeamLead>(teamLeadsFilePath);
                });
                services.AddSingleton<HrManager>();
                services.AddSingleton<HrDirector>(); 
            }).Build();
    
        await host.RunAsync();
        await Log.CloseAndFlushAsync();
    }
}