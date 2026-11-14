namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendConfirmationEmailAsync(string email, string name, string confirmationLink, CancellationToken ct = default);

        Task<Result> SendPasswordResetEmailAsync(string email, string name, string token, CancellationToken ct = default);

        Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);

        Task<Result> SendEmailChangeWarningAsync(string email, string name, CancellationToken ct = default);

        Task<Result> SendEmailChangeConfirmationAsync(string email, string name, string link, CancellationToken ct = default);

        Task<Result> SendActivationReminderEmailAsync(string email, string name, string link, CancellationToken ct = default);

    }
}