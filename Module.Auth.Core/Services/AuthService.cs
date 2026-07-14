using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Module.Auth.Core.DBOs;
using Module.Auth.Core.DTOs;
using Module.Auth.Core.DBContext;
using Shared.Infrastructure.Helpers;
using Shared.Infrastructure.Models;
using Shared.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;

namespace Module.Auth.Core.Services;

public class AuthService : IAuthService
{
    private readonly IAuthDBContext _db;
    private readonly IEmailService _email;
    private readonly IGuidService _guid;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public AuthService(
        IAuthDBContext db,
        IEmailService email,
        IGuidService guid,
        IConfiguration config,
         IMapper mapper)
    {
        _db     = db;
        _email  = email;
        _guid   = guid;
        _config = config;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MessageResponseDto>> SendOtpAsync(SendOtpRequestDto dto)
    {
        var traceId = _guid.NewGuid();
        var code = new Random().Next(100000, 999999).ToString();
        var expiresAt = DateTime.UtcNow.AddMinutes(10);

        await _db.CreateOtpAsync(dto.Email, code, "Register", expiresAt);
        await _email.SendOtpAsync(dto.Email, code);

        var result = new MessageResponseDto { Message = "OTP sent to your email." };
        return result.ToSuccessResponse(traceId);
    }

    public async Task<ApiResponse<MessageResponseDto>> RegisterAsync(RegisterRequestDto dto)
    {
        var traceId = _guid.NewGuid();

        // Check if user already exists
        var existing = await _db.GetUserByEmailAsync(dto.Email);
        if (existing is not null)
        {
            var err = new MessageResponseDto { Message = "Email already registered." };
            return err.ToErrorResponse(traceId, "Email already registered.", "01");
        }

        // Verify OTP
        var otp = await _db.GetLatestOtpAsync(dto.Email, "Register");
        if (otp is null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow || otp.Code != dto.OtpCode)
        {
            var err = new MessageResponseDto { Message = "Invalid or expired OTP." };
            return err.ToErrorResponse(traceId, "Invalid or expired OTP.", "02");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _db.CreateUserAsync(dto.Username, dto.Email, passwordHash, "Patient");
        await _db.MarkOtpUsedAsync(otp.OtpId);

        var result = new MessageResponseDto { Message = "Registration successful." };
        return result.ToSuccessResponse(traceId);
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var traceId = _guid.NewGuid();
        var user = await _db.GetUserByEmailAsync(dto.Email);

        if (user is null)
        {
            var err = new LoginResponseDto();
            return err.ToErrorResponse(traceId, "Invalid email or password.", "01");
        }

        if (user.IsLocked)
        {
            var err = new LoginResponseDto();
            return err.ToErrorResponse(traceId, "Account locked. Contact support.", "03");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            await _db.IncrementLoginAttemptsAsync(dto.Email);
            var err = new LoginResponseDto();
            return err.ToErrorResponse(traceId, "Invalid email or password.", "01");
        }

        await _db.ResetLoginAttemptsAsync(dto.Email);
        var token = GenerateJwtToken(user);

        var result = _mapper.Map<LoginResponseDto>(user);
        return.Toeken = token;
    }

    public async Task<ApiResponse<MessageResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
    {
        var traceId = _guid.NewGuid();

        // Anti-enumeration — always return same message
        var user = await _db.GetUserByEmailAsync(dto.Email);
        if (user is not null)
        {
            var code = new Random().Next(100000, 999999).ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(10);
            await _db.CreateOtpAsync(dto.Email, code, "ResetPassword", expiresAt);
            await _email.SendOtpAsync(dto.Email, code);
        }

        var result = new MessageResponseDto
        {
            Message = "If this email exists, a reset OTP has been sent."
        };
        return result.ToSuccessResponse(traceId);
    }

    public async Task<ApiResponse<MessageResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        var traceId = _guid.NewGuid();

        var otp = await _db.GetLatestOtpAsync(dto.Email, "ResetPassword");
        if (otp is null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow || otp.Code != dto.OtpCode)
        {
            var err = new MessageResponseDto { Message = "Invalid or expired OTP." };
            return err.ToErrorResponse(traceId, "Invalid or expired OTP.", "02");
        }

        var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.UpdatePasswordAsync(dto.Email, newHash);
        await _db.MarkOtpUsedAsync(otp.OtpId);

        var result = new MessageResponseDto { Message = "Password reset successful." };
        return result.ToSuccessResponse(traceId);
    }

    private string GenerateJwtToken(UserDbo user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Role,           user.Role)
        };
        var token = new JwtSecurityToken(
            issuer:            _config["Jwt:Issuer"],
            audience:          _config["Jwt:Audience"],
            claims:            claims,
            expires:           DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
