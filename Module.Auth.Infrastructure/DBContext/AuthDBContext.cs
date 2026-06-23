using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Module.Auth.Core.DBOs;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;

namespace Module.Auth.Infrastructure.DBContext;

public class AuthDBContext : BaseDBContext, IAuthDBContext
{
    public AuthDBContext(IOptions<DatabaseConnection> options) : base(options) { }

    public async Task<UserDbo?> GetUserByEmailAsync(string email) =>
        await QuerySingleAsync(
            "sp_GetUserByEmail",
            [new SqlParameter("@Email", email)],
            MapUser);

    public async Task<OtpDbo?> GetLatestOtpAsync(string email, string purpose) =>
        await QuerySingleAsync(
            "sp_GetLatestOtp",
            [
                new SqlParameter("@Email", email),
                new SqlParameter("@Purpose", purpose)
            ],
            r => new OtpDbo
            {
                OtpId     = r.GetInt32(r.GetOrdinal("OtpId")),
                Email     = r.GetString(r.GetOrdinal("Email")),
                Code      = r.GetString(r.GetOrdinal("Code")),
                Purpose   = r.GetString(r.GetOrdinal("Purpose")),
                ExpiresAt = r.GetDateTime(r.GetOrdinal("ExpiresAt")),
                IsUsed    = r.GetBoolean(r.GetOrdinal("IsUsed")),
                CreatedAt = r.GetDateTime(r.GetOrdinal("CreatedAt"))
            });

    public async Task CreateUserAsync(
        string username, string email,
        string passwordHash, string role) =>
        await ExecuteAsync("sp_CreateUser",
        [
            new SqlParameter("@Username",     username),
            new SqlParameter("@Email",        email),
            new SqlParameter("@PasswordHash", passwordHash),
            new SqlParameter("@Role",         role)
        ]);

    public async Task CreateOtpAsync(
        string email, string code,
        string purpose, DateTime expiresAt) =>
        await ExecuteAsync("sp_CreateOtp",
        [
            new SqlParameter("@Email",     email),
            new SqlParameter("@Code",      code),
            new SqlParameter("@Purpose",   purpose),
            new SqlParameter("@ExpiresAt", expiresAt)
        ]);

    public async Task MarkOtpUsedAsync(int otpId) =>
        await ExecuteAsync("sp_MarkOtpUsed",
            [new SqlParameter("@OtpId", otpId)]);

    public async Task UpdatePasswordAsync(string email, string newPasswordHash) =>
        await ExecuteAsync("sp_UpdatePassword",
        [
            new SqlParameter("@Email",           email),
            new SqlParameter("@NewPasswordHash", newPasswordHash)
        ]);

    public async Task IncrementLoginAttemptsAsync(string email) =>
        await ExecuteAsync("sp_IncrementLoginAttempts",
            [new SqlParameter("@Email", email)]);

    public async Task ResetLoginAttemptsAsync(string email) =>
        await ExecuteAsync("sp_ResetLoginAttempts",
            [new SqlParameter("@Email", email)]);

    private static UserDbo MapUser(SqlDataReader r) => new()
    {
        UserId        = r.GetInt32(r.GetOrdinal("UserId")),
        Username      = r.GetString(r.GetOrdinal("Username")),
        Email         = r.GetString(r.GetOrdinal("Email")),
        PasswordHash  = r.GetString(r.GetOrdinal("PasswordHash")),
        Role          = r.GetString(r.GetOrdinal("Role")),
        IsLocked      = r.GetBoolean(r.GetOrdinal("IsLocked")),
        LoginAttempts = r.GetInt32(r.GetOrdinal("LoginAttempts")),
        CreatedAt     = r.GetDateTime(r.GetOrdinal("CreatedAt"))
    };
}
