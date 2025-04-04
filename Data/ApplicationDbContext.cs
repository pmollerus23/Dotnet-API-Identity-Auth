using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication.Models.Users;

namespace WebApplication.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<UserFriendShip> UserFriendships { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            // entity.HasKey(e => e.Id);

            entity.HasKey(e => e.Id);
            // .IsRequired();

            entity.Property(e => e.Id).IsRequired();

            // Configure one-to-one relationship with IdentityUser
            entity.HasOne(au => au.IdentityUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(au => au.IdentityUserId)
                .IsRequired();

            entity.Property(e => e.FirstName)
                .HasMaxLength(20);

            entity.Property(e => e.LastName)
                .HasMaxLength(40);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(40);
        });

        builder.Entity<UserFriendShip>()
        .HasKey(f => new { f.User1Id, f.User2Id });
        
        builder.Entity<UserFriendShip>().ToTable(t => t
        .HasCheckConstraint("CK_Friendship_User1Id_LessThan_User2Id", "User1Id < User2Id"));
    }
}