using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // private readonly IUserService _userService;
    // private readonly ILogger<UserController> _logger;
    //
    // public UserController(ILogger<UserController> logger, IUserService userService)
    // {
    //     _logger = logger;
    //     _userService = userService;
    // }
    
    public IActionResult GetUserId()
    {
        // Access the claims from the current HttpContext
        var userIdClaim = User.FindFirst(ClaimTypes.Name);

        if (userIdClaim != null)
        {
            // The User ID (usually GUID or string) from the token's claim
            return Ok(new { UserId = userIdClaim.Value });
        }
        else
        {
            return Unauthorized("User ID claim not found.");
        }
    }

}