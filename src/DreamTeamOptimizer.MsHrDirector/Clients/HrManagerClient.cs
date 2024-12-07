using System.Web;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace DreamTeamOptimizer.MsHrDirector.Clients;

public class HrManagerClient : IHrManagerClient
{
    public const string ServiceName = "ms-hr-manager";

    private readonly ILogger<HrManagerClient> _logger;
    private readonly HttpClient _httpClient;

    public HrManagerClient(ILogger<HrManagerClient> logger, IDiscoveryClient discoveryClient)
    {
        _logger = logger;

        var handler = new DiscoveryHttpClientHandler(discoveryClient);
        _httpClient = new HttpClient(handler, false);
    }

    public void ConductHackathon(int hackathonId)
    {
        _logger.LogInformation("send request to ms-hr-manager to conduct hackathon");

        var task = _httpClient.GetAsync($"http://{ServiceName}/api/v1/hackathons/{hackathonId}");
        task.Wait();
        var response = task.Result;

        if (!response.IsSuccessStatusCode)
            throw new HttpStatusException(response.StatusCode, $"HR manager return error: {response.ReasonPhrase}");
    }
}