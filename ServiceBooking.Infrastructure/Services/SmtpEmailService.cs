using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceBooking.Application.Common.Interfaces;

namespace ServiceBooking.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        string senderEmail = _configuration["SMTP_SENDER_EMAIL"]
            ?? throw new InvalidOperationException("SMTP_SENDER_EMAIL is not configured.");
        string appPassword = _configuration["SMTP_PASSWORD"]
            ?? throw new InvalidOperationException("SMTP_PASSWORD is not configured.");

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };

        mailMessage.To.Add(to);

        using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(senderEmail, appPassword),
            EnableSsl = true
        };

        try
        {
            // SendMailAsync with CancellationToken
            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Recipient}.", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}: {Message}", to, ex.Message);
            throw; // Rethrow if you want the queue message to retry in Service Bus
        }
    }
}