namespace Module.Auth.Core.DBOs;

public class UserDbo
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public int LoginAttempts { get; set; }
    public DateTime CreatedAt { get; set; }
}