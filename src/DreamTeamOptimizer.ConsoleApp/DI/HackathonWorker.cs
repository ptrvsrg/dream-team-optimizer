using DreamTeamOptimizer.Core.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.DI;

public class HackathonWorker(
    IConfiguration configuration,
    IHackathonService hackathonService,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            Log.Information("Hackathons started");

            var hackathonsCount = int.Parse(configuration["HACKATHON_HACKATHON_COUNT"]!);
            var totalHarmonicity = 0.0;
            var averageHarmonicity = 0.0;

            for (int i = 0; i < hackathonsCount; i++)
            {
                double harmonicity;
                try
                {
                    harmonicity = hackathonService.Conduct();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    continue;
                }

                totalHarmonicity += harmonicity;
                averageHarmonicity = totalHarmonicity / i + 1;
                Log.Information(
                    $"Hackathon {i + 1}: harmonicity={harmonicity:F5}, average_harmonicity={averageHarmonicity:F5}");
            }

            Log.Information($"Average harmonicity across all hackathons: {averageHarmonicity:F5}");
            appLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Hackathons finished");
        return Task.CompletedTask;
    }
}