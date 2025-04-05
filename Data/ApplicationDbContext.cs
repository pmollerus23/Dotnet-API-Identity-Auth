using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication.Models.Users;

namespace WebApplication.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Friendship> Friendships { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.IdentityUserId);
     
            // Configure one-to-one relationship with IdentityUser
            entity.HasOne(au => au.IdentityUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(au => au.IdentityUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.FirstName)
                .HasMaxLength(20);

            entity.Property(e => e.LastName)
                .HasMaxLength(40);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(40);
        });

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.FriendshipsAsUser1)
            .WithOne(f => f.User1)
            .HasForeignKey(f => f.User1Id);

    builder.Entity<ApplicationUser>()
        .HasMany(u => u.FriendshipsAsUser2)
        .WithOne(f => f.User2)
        .HasForeignKey(f => f.User2Id);

        // Composite key for Friendship table
        builder.Entity<Friendship>()
            .HasKey(f => new { f.User1Id, f.User2Id });

        // Relationship configuration: User1
        builder.Entity<Friendship>()
            .HasOne(f => f.User1)
            .WithMany(u => u.FriendshipsAsUser1)
            .HasForeignKey(f => f.User1Id)
            .OnDelete(DeleteBehavior.Restrict);  // Adjust based on your needs

        // Relationship configuration: User2
        builder.Entity<Friendship>()
            .HasOne(f => f.User2)
            .WithMany(u => u.FriendshipsAsUser2)
            .HasForeignKey(f => f.User2Id)
            .OnDelete(DeleteBehavior.Restrict);  // Adjust based on your needs
        
        builder.Entity<Friendship>().ToTable(t => t
        .HasCheckConstraint("CK_Friendship_User1Id_LessThan_User2Id", "User1Id < User2Id"));
    }
}