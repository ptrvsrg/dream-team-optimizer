using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using DreamTeamOptimizer.MsHrDirector.Services.Mappers;
using Microsoft.Extensions.Caching.Memory;
using Math = DreamTeamOptimizer.Core.Helpers.Math;
using Satisfaction = DreamTeamOptimizer.Core.Persistence.Entities.Satisfaction;

namespace DreamTeamOptimizer.MsHrDirector.Services;

public class HackathonService(
    ILogger<HackathonService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IMemoryCache cache,
    IHackathonRepository hackathonRepository,
    IHackathonStartedPublisher hackathonStartedPublisher,
    ISatisfactionService satisfactionService) : IHackathonService
{
    private readonly TimeSpan _hackathonTimeout = TimeSpan.FromMinutes(3);

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

        try
        {
            // Create session
            logger.LogDebug("create new session");
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_hackathonTimeout)
                .RegisterPostEvictionCallback(SessionExpiredCallback);
            cache.Set(hackathon.Id, true, cacheEntryOptions);

            // Make HR manager to conduct hackathon
            logger.LogDebug("publish message to HR manager");
            hackathonStartedPublisher.StartHackathon(hackathon.Id);
        }
        catch (Exception)
        {
            logger.LogWarning("failed to conduct hackathon");
            hackathon.Status = Core.Persistence.Entities.HackathonStatus.FAILED;
            hackathonRepository.Update(hackathon);

            throw;
        }

        return HackathonMapper.ToModelSimple(hackathon);
    }

    public void SaveResult(List<Team> teams, List<WishList> teamLeadsWishlists, List<WishList> juniorsWishlists,
        int hackathonId)
    {
        logger.LogInformation("hackathon completed");

        // Find hackathon
        logger.LogDebug("find hackathon by id");
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
            satisfactionService.Evaluate(teams, teamLeadsWishlists, juniorsWishlists);

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

        if (hackathon.Status != Core.Persistence.Entities.HackathonStatus.COMPLETED)
        {
            hackathon.Result = 0.0;
            hackathon.WishLists = new List<Core.Persistence.Entities.WishList>();
            hackathon.Satisfactions = new List<Satisfaction>();
            hackathon.Teams = new List<Core.Persistence.Entities.Team>();
        }

        return HackathonMapper.ToModel(hackathon);
    }

    private void SessionExpiredCallback(object key, object value, EvictionReason reason, object state)
    {
        if (reason != EvictionReason.Expired) return;

        logger.LogInformation("session expired");

        var scope = serviceScopeFactory.CreateScope();
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