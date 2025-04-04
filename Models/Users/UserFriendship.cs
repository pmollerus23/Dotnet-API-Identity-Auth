using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models.Users;

public class UserFriendShip
{
    [Key]
    [Column(Order = 0)]
    public string User1Id { get; set; }
    [Key]
    [Column(Order = 1)]
    public string User2Id { get; set; }
    [Required]
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [ForeignKey("User1Id")]
    public virtual ApplicationUser User1 { get; set; }
    [ForeignKey("User2Id")]
    public virtual ApplicationUser User2 { get; set; }
}

public enum FriendshipStatus
{
    Pending,
    Approved,
    Declined,
    Blocked
}