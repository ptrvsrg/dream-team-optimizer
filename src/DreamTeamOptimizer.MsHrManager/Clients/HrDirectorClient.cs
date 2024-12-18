using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrManager.Interfaces.Clients;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace DreamTeamOptimizer.MsHrManager.Clients;

public class HrDirectorClient : IHrDirectorClient
{
    public const string ServiceName = "ms-hr-director";

    private readonly ILogger<HrDirectorClient> _logger;
    private readonly HttpClient _httpClient;

    public HrDirectorClient(ILogger<HrDirectorClient> logger, IDiscoveryClient discoveryClient)
    {
        _logger = logger;

        var handler = new DiscoveryHttpClientHandler(discoveryClient);
        _httpClient = new HttpClient(handler, false);
    }

    public void SaveResult(HackathonResult result, int hackathonId)
    {
        _logger.LogInformation("send hackathon result to ms-hr-director");

        var task = _httpClient.PostAsJsonAsync(
            $"http://{ServiceName}/api/v1/hackathons/webhook?hackathonId={hackathonId}", result);
        task.Wait();
        var response = task.Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpStatusException(response.StatusCode, $"HR director return error: {response.ReasonPhrase}");
        }
    }
}