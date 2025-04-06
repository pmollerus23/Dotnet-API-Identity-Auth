using WebApplication.Models.Users;

namespace WebApplication.DTOs;

public record FriendshipDto()
{
    public required string User1Email {get; set;}
    public required string User2Email {get; set;}
    public required FriendshipStatus User1Status {get; set;}
    public required FriendshipStatus User2Status {get;set;}
    public DateTime? UpdatedAt {get; set;}
}