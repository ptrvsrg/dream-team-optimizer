using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.DI;

public class HackathonStatWorker(
    IConfiguration config,
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
                Execute();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            appLifetime.StopApplication();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private void Execute()
    {
        var hackathonId = config.GetValue<int?>("hackathonId");
        if (hackathonId == null)
        {
            Log.Information("no hackathon ID provided");
            return;
        }

        var hackathon = hackathonService.FindById(hackathonId.Value);
        if (hackathon == null)
        {
            Log.Information($"hackathon #{hackathonId} not found");
            return;
        }

        var juniorsStat = hackathon.Employees
            .Where(e => e.Grade == Grade.JUNIOR)
            .Select(e => $"\t#{e.Id} {e.Name}")
            .Aggregate((current, next) => current + "\n" + next);
        var teamleadsStat = hackathon.Employees
            .Where(e => e.Grade == Grade.TEAM_LEAD)
            .Select(e => $"\t#{e.Id} {e.Name}")
            .Aggregate((current, next) => current + "\n" + next);
        var wishListsStat = hackathon.WishLists
            .Select(w => $"\t#{w.EmployeeId} - [{w.DesiredEmployees.Select(id => $"#{id}").Aggregate((current, next) => current + ", " + next)}]")
            .Aggregate((current, next) => current + "\n" + next);
        var teamsStat = hackathon.Teams
            .Select(t => $"\t#{t.TeamLeadId} - #{t.JuniorId}")
            .Aggregate((current, next) => current + "\n" + next);

        var statistics = $"Found hackathon #{hackathon.Id}:\n" +
                         $"Status: \n\t{hackathon.Status.ToString().ToLower()}\n" +
                         $"Harmonicity: \n\t{hackathon.Result:F5}\n" +
                         $"Juniors: \n{juniorsStat}\n" +
                         $"Team leads: \n{teamleadsStat}\n" +
                         $"Wish lists: \n{wishListsStat}\n" +
                         $"Teams: \n{teamsStat}";
                         
        Log.Information(statistics);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}