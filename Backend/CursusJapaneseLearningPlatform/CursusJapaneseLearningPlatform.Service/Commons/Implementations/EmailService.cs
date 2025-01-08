using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;

namespace CursusJapaneseLearningPlatform.Service.Commons.Implementations;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _senderEmail = configuration["EmailSettings:Sender"]
            ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email Sender is not configured.");
        var password = configuration["EmailSettings:Password"];
        var host = configuration["EmailSettings:Host"];
        var port = int.Parse(configuration["EmailSettings:Port"]
            ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email port is not configured."));

        _smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_senderEmail, password)
        };

        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body)
    {
        try
        {
            foreach (var to in toList)
            {
                var mailMessage = new MailMessage(_senderEmail, to, subject, body);
                await _smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
            return false;
        }

        return true;
    }

    public async Task<bool> SendForgotPasswordEmailAsync(string toMail, string subject, string fullName, string username, string resetPasswordUrl)
    {
        try
        {
            // Đường dẫn tới file template HTML trong thư mục API
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ResetPasswordEmail.html");

            // Đọc nội dung từ file template
            string body = await File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholder trong nội dung email
            body = body
                .Replace("[[FULL_NAME]]", fullName)
                .Replace("[[USERNAME]]", username)
                .Replace("[[URL]]", resetPasswordUrl);

            var mailMessage = new MailMessage(_senderEmail, toMail, subject, body)
            {
                IsBodyHtml = true // Thiết lập nội dung là HTML
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
            return false;
        }

        return true;
    }

    public async Task<bool> SendVerifyEmailAsync(string toMail, string subject, string fullName, string username, string verificationUrl)
    {
        try
        {
            // Đường dẫn tới file template HTML trong thư mục API
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplate.html");

            // Đọc nội dung từ file template
            string body = await File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholder trong nội dung email
            body = body
                .Replace("[[FULL_NAME]]", fullName)
                .Replace("[[USERNAME]]", username)
                .Replace("[[URL]]", verificationUrl);

            var mailMessage = new MailMessage(_senderEmail, toMail, subject, body)
            {
                IsBodyHtml = true // Thiết lập nội dung là HTML
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
            return false;
        }

        return true;
    }

}
