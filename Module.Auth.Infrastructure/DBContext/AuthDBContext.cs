using Microsoft.Extensions.Options;
using Module.Auth.Core.DBOs;
using Module.Auth.Core.DBContext;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;

namespace Module.Auth.Infrastructure.DBContext;

public class AuthDBContext : BaseDBContext, IAuthDBContext
{
    public AuthDBContext(IOptions<DatabaseConnection> options)
        : base(options)
    {
    }

    public async Task<UserDbo?> GetUserByEmailAsync(string email)
    {
        return await QuerySingleAsync<UserDbo>(
            "sp_GetUserByEmail",
            new
            {
                Email = email
            });
    }

    public async Task<OtpDbo?> GetLatestOtpAsync(string email, string purpose)
    {
        return await QuerySingleAsync<OtpDbo>(
            "sp_GetLatestOtp",
            new
            {
                Email = email,
                Purpose = purpose
            });
    }

    public async Task CreateUserAsync(
        string username,
        string email,
        string passwordHash,
        string role)
    {
        await ExecuteAsync(
            "sp_CreateUser",
            new
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            });
    }

    public async Task CreateOtpAsync(
        string email,
        string code,
        string purpose,
        DateTime expiresAt)
    {
        await ExecuteAsync(
            "sp_CreateOtp",
            new
            {
                Email = email,
                Code = code,
                Purpose = purpose,
                ExpiresAt = expiresAt
            });
    }

    public async Task MarkOtpUsedAsync(int otpId)
    {
        await ExecuteAsync(
            "sp_MarkOtpUsed",
            new
            {
                OtpId = otpId
            });
    }

    public async Task UpdatePasswordAsync(
        string email,
        string newPasswordHash)
    {
        await ExecuteAsync(
            "sp_UpdatePassword",
            new
            {
                Email = email,
                NewPasswordHash = newPasswordHash
            });
    }

    public async Task IncrementLoginAttemptsAsync(string email)
    {
        await ExecuteAsync(
            "sp_IncrementLoginAttempts",
            new
            {
                Email = email
            });
    }

    public async Task ResetLoginAttemptsAsync(string email)
    {
        await ExecuteAsync(
            "sp_ResetLoginAttempts",
            new
            {
                Email = email
            });
    }
}