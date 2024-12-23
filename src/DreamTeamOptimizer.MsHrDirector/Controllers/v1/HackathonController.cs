using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public ActionResult<HackathonSimple> Create()
    {
        var hackathon = hackathonService.Create();
        return Created($"/api/v1/hackathons/{hackathon.Id}", hackathon);
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
    public IActionResult GetById(int id)
    {
        var hackathon = hackathonService.GetById(id);
        return Ok(hackathon);
    }

    /// <summary>Calculate average harmonicity</summary>
    /// <response code="200">Returns the average harmonicity</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [Route("average-harmonic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CalculateAverageHarmonicity()
    {
        var averageHarmonicity = hackathonService.CalculateAverageHarmonicity();
        return Ok(averageHarmonicity);
    }
}