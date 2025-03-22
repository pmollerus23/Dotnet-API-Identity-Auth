namespace WebApplication.DTOs;

public record AppUserDto()
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PrefName { get; set; }
    public string? PhoneNumber { get; set; }
}