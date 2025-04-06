using WebApplication.DTOs;
using WebApplication.Models.Users;

namespace WebApplication.Services;

public interface IApplicationUserService
{
    Task<ApplicationUser> CreateAppUserAsync(AppUserDto applicationUserDto, string userId);
    Task<ApplicationUser> GetByIdentityUserIdAsync(string identityId);
    Task<ApplicationUser> UpdateAppUserAsync(AppUserDto applicationUser, string userId);
    Task<ICollection<FriendshipDto>> GetUserFriendships(string identityId);
    Task<int?> AddFriendAsync(string senderId, string recipientEmail);

}