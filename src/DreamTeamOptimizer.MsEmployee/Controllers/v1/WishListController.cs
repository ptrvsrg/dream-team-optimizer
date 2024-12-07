using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsEmployee.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DreamTeamOptimizer.MsEmployee.Controllers.v1;

[ApiController]
[Route("/api/v1/wishlists")]
public class WishListController(IWishListService wishListService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public WishList GetById([FromQuery] int[] desiredEmployeeIds) => wishListService.GetWishlist(desiredEmployeeIds.ToList());
}