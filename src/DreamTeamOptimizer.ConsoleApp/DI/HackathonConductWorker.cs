using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.DI;

public class HackathonConductWorker(
    IOptions<Config.ApplicationOptions> config,
    IHackathonService hackathonService,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            Execute();
            appLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private void Execute()
    {
        var hackathonsCount = config.Value.HackathonCount;
        var totalHarmonicity = 0.0;
        var averageHarmonicity = 0.0;

        for (int i = 0; i < hackathonsCount; i++)
        {
            Hackathon hackathon;
            try
            {
                hackathon = hackathonService.Conduct();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                continue;
            }

            totalHarmonicity += hackathon.Result;
            averageHarmonicity = totalHarmonicity / (i + 1);
            Log.Information(
                $"hackathon #{hackathon.Id}: harmonicity={hackathon.Result:F5}, average_harmonicity={averageHarmonicity:F5}");
        }

        Log.Information($"average harmonicity across all hackathons: {averageHarmonicity:F5}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}