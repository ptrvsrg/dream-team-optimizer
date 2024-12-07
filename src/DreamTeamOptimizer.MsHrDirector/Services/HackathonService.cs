using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using DreamTeamOptimizer.MsHrDirector.Services.Mappers;
using Microsoft.Extensions.Caching.Memory;
using Math = DreamTeamOptimizer.Core.Helpers.Math;

namespace DreamTeamOptimizer.MsHrDirector.Services;

public class HackathonService(
    ILogger<HackathonService> logger,
    IServiceProvider serviceProvider,
    IMemoryCache cache,
    IHackathonRepository hackathonRepository,
    IHrManagerClient hrManagerClient,
    ISatisfactionService satisfactionService) : IHackathonService
{
    private readonly TimeSpan _hackathonTimeout = TimeSpan.FromMinutes(1);

    public HackathonSimple Create()
    {
        logger.LogInformation("create new hackathon");

        // Create hackathon
        logger.LogDebug("save new hackathon to database");
        var hackathon = new Core.Persistence.Entities.Hackathon
        {
            Status = Core.Persistence.Entities.HackathonStatus.IN_PROCESSING,
            Result = 0.0
        };
        hackathonRepository.Create(hackathon);

        // Create session
        logger.LogDebug("create new session");
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_hackathonTimeout)
            .RegisterPostEvictionCallback(SessionExpiredCallback);
        cache.Set(hackathon.Id, true, cacheEntryOptions);

        // Make HR manager to conduct hackathon
        logger.LogDebug("make hr manager to conduct hackathon");
        try
        {
            hrManagerClient.ConductHackathon(hackathon.Id);
        }
        catch (Exception)
        {
            logger.LogWarning("failed to conduct hackathon");
            hackathon.Status = Core.Persistence.Entities.HackathonStatus.FAILED;
            hackathonRepository.Update(hackathon);

            throw;
        }

        return HackathonMapper.ToModelSimple(hackathonRepository.FindById(hackathon.Id)!);
    }

    public void SaveResult(HackathonResult result, int hackathonId)
    {
        logger.LogInformation("save hackathon result");

        // Find hackathon
        var hackathon = hackathonRepository.FindById(hackathonId);
        if (hackathon == null)
            throw new HttpStatusException(HttpStatusCode.NotFound, $"No hackathon #{hackathonId} found");

        // Check session
        logger.LogDebug("check session");
        if (!cache.TryGetValue(hackathonId, out bool _))
            throw new HttpStatusException(HttpStatusCode.NotFound, $"Hackathon #{hackathonId} has expired");

        // Calculate satisfactions
        logger.LogDebug("calculate satisfactions");
        var satisfactions =
            satisfactionService.Evaluate(result.Teams, result.TeamLeadsWishlists, result.JuniorsWishlists);

        // Calculate harmonic mean
        logger.LogDebug("calculate harmonic mean");
        var satisfactionRanks = satisfactions.Select(s => s.Rank).ToList();
        var harmonicMean = Math.CalculateHarmonicMean(satisfactionRanks);
        hackathon.Result = harmonicMean;

        // Save result
        logger.LogDebug("save result to database");
        hackathon.Status = Core.Persistence.Entities.HackathonStatus.COMPLETED;
        hackathonRepository.Update(hackathon);

        // Delete session
        logger.LogDebug("delete session");
        cache.Remove(hackathonId);
    }

    public AverageHarmonicity CalculateAverageHarmonicity()
    {
        logger.LogInformation("calculate average harmonicity");
        var result = hackathonRepository.FindAverageResult();
        return new AverageHarmonicity(result);
    }

    public Hackathon GetById(int id)
    {
        logger.LogInformation($"find hackathon #{id}");

        var hackathon = hackathonRepository.FindById(id);
        if (hackathon == null) throw new HttpStatusException(HttpStatusCode.NotFound, $"No hackathon #{id} found");

        return HackathonMapper.ToModel(hackathon);
    }

    private void SessionExpiredCallback(object key, object value, EvictionReason reason, object state)
    {
        if (reason != EvictionReason.Expired) return;

        logger.LogInformation("session expired");

        var scope = serviceProvider.CreateScope();
        var scopedHackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();

        var hackathon = scopedHackathonRepository.FindById(key);
        if (hackathon == null)
        {
            logger.LogWarning($"hackathon #{key} not found");
            return;
        }

        hackathon.Status = Core.Persistence.Entities.HackathonStatus.FAILED;
        scopedHackathonRepository.Update(hackathon);
    }
}