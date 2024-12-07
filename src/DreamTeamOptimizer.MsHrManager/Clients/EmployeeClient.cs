using System.Net;
using System.Web;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrManager.Interfaces.Clients;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace DreamTeamOptimizer.MsHrManager.Clients;

public class EmployeeClient : IEmployeeClient
{
    public const string ServiceName = "ms-employee";

    private readonly ILogger<EmployeeClient> _logger;
    private readonly HttpClient _httpClient;

    public EmployeeClient(ILogger<EmployeeClient> logger, IDiscoveryClient discoveryClient)
    {
        _logger = logger;

        var handler = new DiscoveryHttpClientHandler(discoveryClient);
        _httpClient = new HttpClient(handler, false);
    }

    public WishList Vote(int employeeId, List<int> desiredEmployeeIds)
    {
        _logger.LogInformation($"Send vote request to employee {employeeId}");

        var builder = new UriBuilder($"http://{ServiceName}-{employeeId}");
        builder.Port = -1;
        builder.Path = "/api/v1/wishlists";

        var query = HttpUtility.ParseQueryString(builder.Query);
        desiredEmployeeIds.ForEach(id => query.Add("desiredEmployeeIds", id.ToString()));
        builder.Query = query.ToString();

        var task = _httpClient.GetAsync(builder.ToString());
        task.Wait();

        var response = task.Result;
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpStatusException(HttpStatusCode.InternalServerError,
                $"Employee with ID {employeeId} returns error: {response.ReasonPhrase}");
        }

        var contentTask = response.Content.ReadFromJsonAsync<WishList>();
        contentTask.Wait();

        var result = contentTask.Result;
        if (result == null)
        {
            throw new HttpStatusException(HttpStatusCode.BadRequest,
                $"Employee with ID {employeeId} returns empty wish list");
        }

        return result;
    }
}