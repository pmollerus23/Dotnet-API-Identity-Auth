using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs;
using WebApplication.Models.Users;
using Microsoft.Data.Sqlite;
using WebApplication.Exceptions;

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

    public async Task<ICollection<FriendshipDto>> GetUserFriendships(string identityId)
    {
        var friends = await _context.Friendships
        .Where(f => f.User1Id == identityId || f.User2Id == identityId)
        .Include(f => f.User1)
        .Include(f => f.User2)
        .Select(f => new FriendshipDto {
            User1Email = f.User1.IdentityUser.Email,
            User2Email = f.User2.IdentityUser.Email,
            User1Status = f.User1Status,
            User2Status = f.User2Status,
            UpdatedAt = f.UpdatedAt
        })
        .ToListAsync();

    return friends;
    }

    public async Task<int?> AddFriendAsync(string senderId, string recipientEmail)
    {
        var existingUser = await _context.ApplicationUsers
        .FirstOrDefaultAsync(u => u.IdentityUser.Email == recipientEmail);
          if (existingUser == null)
    {
        // If recipient doesn't exist, return a not found response
        return null;

    } else if (existingUser.IdentityUserId == senderId) {
        throw new InvalidConstraintException("user object cannot be a friend of itself");
    }

        var result = await _context.Friendships.AddAsync(new Friendship
        {
            User1Id = senderId.CompareTo(existingUser.IdentityUserId) < 0 ? senderId : existingUser.IdentityUserId,
            User2Id = senderId.CompareTo(existingUser.IdentityUserId) < 0 ? existingUser.IdentityUserId : senderId,
            User1Status = senderId.CompareTo(existingUser.IdentityUserId) < 0 ? FriendshipStatus.Approved : FriendshipStatus.Pending,
            User2Status = senderId.CompareTo(existingUser.IdentityUserId) < 0 ? FriendshipStatus.Pending : FriendshipStatus.Approved,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e) when (e.InnerException is SqliteException sqliteEx && sqliteEx.SqliteExtendedErrorCode == 1555)
        {
            throw new FriendshipAlreadyExistsException("This friendship already exists", sqliteEx);
        }
        catch
        {
            throw;
        }
    }
}