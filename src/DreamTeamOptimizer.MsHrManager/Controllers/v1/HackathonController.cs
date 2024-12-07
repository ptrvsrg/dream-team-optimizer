using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DreamTeamOptimizer.MsHrManager.Controllers.v1;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[Route("/api/v1/hackathons")]
public class HackathonController(IHackathonService hackathonService) : ControllerBase
{
    /// <summary>Start conduct new hackathon by ID</summary>
    /// <response code="202">Hackathon started</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [Route("{hackathonId}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public AcceptedResult StartConduct(int hackathonId)
    {
        if (!hackathonService.ExistsById(hackathonId))
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"No hackathon #{hackathonId} found");
        }

        var thread = new Thread(() => hackathonService.Conduct(hackathonId))
        {
            IsBackground = true,
            Name = $"HackathonTask-{hackathonId}"
        };
        thread.Start();
        
        return Accepted();
    }
}