using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.DI;

public class AverageHarmonicWorker(
    IHackathonService hackathonService,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            var averageHarmonicity = hackathonService.CalculateAverageHarmonicity();
            Log.Information($"average harmonicity across all hackathons: {averageHarmonicity:F5}");
            appLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}