using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Shared.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOtpAsync(string toEmail, string otpCode)
    {
        var subject = "Your HomeSampling Verification Code";
        var body = $@"
            <h2>Verification Code</h2>
            <p>Your OTP code is:</p>
            <h1 style='letter-spacing:8px;color:#2563eb'>{otpCode}</h1>
            <p>This code expires in <strong>10 minutes</strong>.</p>
            <p>If you didn't request this, please ignore this email.</p>";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendBookingConfirmationAsync(
        string toEmail, string patientName,
        string testName, DateTime scheduledDate)
    {
        var subject = "Booking Confirmed — HomeSampling";
        var body = $@"
            <h2>Booking Confirmed</h2>
            <p>Dear <strong>{patientName}</strong>,</p>
            <p>Your booking has been confirmed.</p>
            <ul>
                <li><strong>Test:</strong> {testName}</li>
                <li><strong>Date:</strong> {scheduledDate:dddd, MMMM dd yyyy}</li>
                <li><strong>Time:</strong> {scheduledDate:hh:mm tt}</li>
            </ul>
            <p>Our rider will visit your address on the scheduled date.</p>";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendContactEmailAsync(
        string fromName, string fromEmail, string message)
    {
        var subject = $"New Contact Form Message from {fromName}";
        var body = $@"
            <h2>New Contact Message</h2>
            <p><strong>From:</strong> {fromName}</p>
            <p><strong>Email:</strong> {fromEmail}</p>
            <p><strong>Message:</strong></p>
            <p>{message}</p>";

        var adminEmail = _config["Email:AdminEmail"]!;
        await SendEmailAsync(adminEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["Email:Host"],
            int.Parse(_config["Email:Port"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _config["Email:Username"],
            _config["Email:Password"]);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
