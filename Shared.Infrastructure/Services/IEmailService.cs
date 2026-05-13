namespace Shared.Infrastructure.Services;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otpCode);
    Task SendBookingConfirmationAsync(string toEmail, string patientName, string testName, DateTime scheduledDate);
    Task SendContactEmailAsync(string fromName, string fromEmail, string message);
}