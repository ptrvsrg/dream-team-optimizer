using DreamTeamOptimizer.Core.Entities;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.HostedServices;

public class HackathonWorker: BackgroundService
{
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly Hackathon _hackathon;
    private readonly CommandLineOptions _commandLineOptions;

    public HackathonWorker(
        IHostApplicationLifetime hostLifetime, Hackathon hackathon, CommandLineOptions commandLineOptions)
    {
        _hostLifetime = hostLifetime;
        _hackathon = hackathon;
        _commandLineOptions = commandLineOptions;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var totalHarmonicity = 0.0;
        var hackathonNum = 0;
        var lockObj = new object();

        Parallel.For(0, _commandLineOptions.HackathonsCount, new ParallelOptions { MaxDegreeOfParallelism = _commandLineOptions.ThreadCount }, _ =>
        {
            var harmonicity = _hackathon.Conduct();
            
            lock (lockObj)
            {
                totalHarmonicity += harmonicity;
                hackathonNum++;
                var averageHarmonicity = totalHarmonicity / hackathonNum;
                Log.Information($"Hackathon {hackathonNum}: harmonicity={harmonicity:F5}, average_harmonicity={averageHarmonicity:F5}");
            }
        });

        var averageHarmonicity = totalHarmonicity / _commandLineOptions.HackathonsCount;
        Log.Information($"Average harmonicity across all hackathons: {averageHarmonicity:F5}");
        _hostLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Hackathons started");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    { 
        Log.Information("Hackathons finished");
        return base.StopAsync(cancellationToken);
    }
}