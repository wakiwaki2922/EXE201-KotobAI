namespace CursusJapaneseLearningPlatform.Service.Commons.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body);

    Task<bool> SendVerifyEmailAsync(string toMail, string subject, string fullName, string username, string verificationUrl);

    Task<bool> SendForgotPasswordEmailAsync(string toMail, string subject, string fullName, string username, string resetPasswordUrl);
}
