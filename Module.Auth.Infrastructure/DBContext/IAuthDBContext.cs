using Module.Auth.Core.DBOs;

namespace Module.Auth.Infrastructure.DBContext;

public interface IAuthDBContext
{
    Task<UserDbo?> GetUserByEmailAsync(string email);
    Task<OtpDbo?> GetLatestOtpAsync(string email, string purpose);
    Task CreateUserAsync(string username, string email, string passwordHash, string role);
    Task CreateOtpAsync(string email, string code, string purpose, DateTime expiresAt);
    Task MarkOtpUsedAsync(int otpId);
    Task UpdatePasswordAsync(string email, string newPasswordHash);
    Task IncrementLoginAttemptsAsync(string email);
    Task ResetLoginAttemptsAsync(string email);
}