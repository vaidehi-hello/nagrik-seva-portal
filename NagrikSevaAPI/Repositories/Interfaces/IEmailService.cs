namespace NagrikSevaAPI.Repositories.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(
            string toEmail,
            string userName,
            string verificationLink
        );
    }
}