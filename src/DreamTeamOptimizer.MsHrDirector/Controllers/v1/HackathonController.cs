using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DreamTeamOptimizer.MsHrDirector.Controllers.v1;

[ApiController]
[Route("/api/v1/hackathons")]
[Consumes("application/json")]
[Produces("application/json")]
public class HackathonController(IHackathonService hackathonService) : ControllerBase
{
    /// <summary>Create new hackathon</summary>
    /// <response code="201">Returns the created hackathon</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public HackathonSimple Create()
    {
        return hackathonService.Create();
    }

    /// <summary>Webhook for saving hackathon result</summary>
    /// <response code="200">Successfully saved</response>
    /// <response code="404">Hackathon not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [Route("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public void SaveResult([FromQuery] int hackathonId, [FromBody] HackathonResult result)
    {
        hackathonService.SaveResult(result, hackathonId);
    }

    /// <summary>Get hackathon by id</summary>
    /// <response code="200">Returns the retrieved hackathon</response>
    /// <response code="404">Hackathon not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Hackathon GetById(int id)
    {
        return hackathonService.GetById(id);
    }

    /// <summary>Calculate average harmonicity</summary>
    /// <response code="200">Returns the average harmonicity</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [Route("average-harmonic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public AverageHarmonicity CalculateAverageHarmonicity()
    {
        return hackathonService.CalculateAverageHarmonicity();
    }
}