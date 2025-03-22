using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApplication.Models.Users;

public class ApplicationUser
{

    
    [Key]  // This is the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string IdentityUserId { get; set; }

    [MaxLength(20)] public required string FirstName { get; set; }

    [MaxLength(40)] public required string LastName { get; set; }
    
    [MaxLength(20)] public string? PreferredName { get; set; }

    [MaxLength(40)] public string? PhoneNumber { get; set; }

    public virtual IdentityUser IdentityUser { get; set; }
}