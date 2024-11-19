using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.DI;

public class HackathonWorker(
    IConfiguration configuration,
    IEmployeeService employeeService,
    IHackathonService hackathonService,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            try
            {
                var hackathonsCount = int.Parse(configuration["HACKATHON_HACKATHON_COUNT"]!);
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
                        $"hackathon #{hackathon.Id}: harmonicity={hackathon.Result  :F5}, average_harmonicity={averageHarmonicity:F5}");
                }

                Log.Information($"average harmonicity across all hackathons: {averageHarmonicity:F5}");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            appLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}