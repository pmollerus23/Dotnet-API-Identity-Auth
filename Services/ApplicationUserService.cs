using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs;
using WebApplication.Models.Users;

namespace WebApplication.Services;

public class ApplicationUserService : IApplicationUserService
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser> CreateAppUserAsync(AppUserDto appUserDto, string userId)
    {
        var appUser = new ApplicationUser
        {
            IdentityUserId = userId,
            FirstName = appUserDto.FirstName,
            LastName = appUserDto.LastName,
            PreferredName = appUserDto.PrefName,
            PhoneNumber = appUserDto.PhoneNumber
        };
        _context.ApplicationUsers.Add(appUser);
        await _context.SaveChangesAsync();
        return appUser;
    }

    public async Task<ApplicationUser> GetByIdentityUserIdAsync(string identityId)
    {
        return await _context.ApplicationUsers
            .Include(au => au.IdentityUser)
            .FirstOrDefaultAsync(au => au.IdentityUserId == identityId);
    }

    public async Task<ApplicationUser> UpdateAppUserAsync(AppUserDto appUserDto, string userId)
    {
        // TODO - Use AutoMapper here
        var existingUser = _context.ApplicationUsers.FirstOrDefault(au => au.IdentityUserId == userId);
        if (existingUser != null)
        {
            existingUser.FirstName = appUserDto.FirstName;
            existingUser.LastName = appUserDto.LastName;
            existingUser.PreferredName = appUserDto.PrefName;
            existingUser.PhoneNumber = appUserDto.PhoneNumber;
            _context.ApplicationUsers.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;

        }
        // _context.ApplicationUsers.Update(existingUser);
        return existingUser;
    }

    public async Task<ICollection<UserFriendShip>> GetUserFriendships(string identityId)
    {
        var friends = await _context.UserFriendships
        .Where(f => f.User1Id == identityId || f.User2Id == identityId)
        // .Select(f => f.User1Id == identityId ? f.User2 : f.User1)
        .ToListAsync();

    return friends;
    }
}