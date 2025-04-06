using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs;
using WebApplication.Models.Users;
using WebApplication.Services;
using WebApplication.Exceptions;

namespace WebApplication.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IApplicationUserService _applicationUserService;

    public UserController(IApplicationUserService applicationUserService)
    {
        _applicationUserService = applicationUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetApplicationUser()
    {
        // Access the claims from the current HttpContext
        // var userName = User.FindFirst(ClaimTypes.Name);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Account not found",
                code = "ACCOUNT_NOT_FOUND"
            });
        }

        ApplicationUser? user = await _applicationUserService.GetByIdentityUserIdAsync(userId);
        if (user == null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Profile does not exist for this user",
                code = "PROFILE_NOT_EXIST"
            });
        }

        return Ok(new
        {
            FirstName = user.FirstName,
            LastName = user.LastName
        });
    }

    [HttpPost]
    public async Task<IActionResult> SetApplicationUser(AppUserDto appUserDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Account not found",
                code = "ACCOUNT_NOT_FOUND"
            });
        }
        ApplicationUser? appUser = await _applicationUserService.GetByIdentityUserIdAsync(userId);
        if (appUser == null)
        {
            var result = await _applicationUserService.CreateAppUserAsync(appUserDto, userId);
            return Ok(new
                {
                    FirstName = result.FirstName,
                    LastName = result.LastName
                }
                );
        }
        var result1 = await _applicationUserService.UpdateAppUserAsync(appUserDto, userId);
        return Ok(new
        {
            FirstName = result1.FirstName,
            LastName = result1.LastName
        });
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetUserFriends() {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Account not found",
                code = "ACCOUNT_NOT_FOUND"
            });
        }

        ICollection<Friendship>? friendships = await _applicationUserService
        .GetUserFriendships(userId);

        if (!friendships.Any()) {
            return Ok(new {
                status = "warning",
                message = "User has no friends",
                code = "HAS_NO_FRIENDS"
            });
        }

        return Ok(friendships);
    }

    [HttpPost("friends")]
    public async Task<IActionResult> AddFriend([FromForm] string recipientEmail) 
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Account not found",
                code = "ACCOUNT_NOT_FOUND"
            });
        }

        try {
            var result = await _applicationUserService.AddFriendAsync(userId, recipientEmail);
        } catch (FriendshipAlreadyExistsException)
        {
            return Conflict("Friendship already exists");
        }
        return Ok();
    }
    
}