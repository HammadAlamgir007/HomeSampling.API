using Module.Auth.Core.DTOs;
using Shared.Infrastructure.Models;

namespace Module.Auth.Core.Services;

public interface IAuthService
{
    Task<ApiResponse<MessageResponseDto>> SendOtpAsync(SendOtpRequestDto dto);
    Task<ApiResponse<MessageResponseDto>> RegisterAsync(RegisterRequestDto dto);
    Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<ApiResponse<MessageResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
    Task<ApiResponse<MessageResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto dto);
}