using Application.Common.Interfaces;
using Domain.SharedKernel;

namespace Application.Tests.Fakes;

public sealed class FakeEmailService : IEmailService
{
    public int SentCount { get; private set; }

    /// <summary>When true, every Send* call returns a failure (SMTP down).</summary>
    public bool FailAll { get; set; }

    public Task<Result> SendConfirmationEmailAsync(string email, string name, string confirmationLink, CancellationToken ct = default) => Send();
    public Task<Result> SendPasswordResetEmailAsync(string email, string name, string token, CancellationToken ct = default) => Send();
    public Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default) => Send();
    public Task<Result> SendEmailChangeWarningAsync(string email, string name, CancellationToken ct = default) => Send();
    public Task<Result> SendEmailChangeConfirmationAsync(string email, string name, string link, CancellationToken ct = default) => Send();
    public Task<Result> SendActivationReminderEmailAsync(string email, string name, string link, CancellationToken ct = default) => Send();

    private Task<Result> Send()
    {
        SentCount++;
        return Task.FromResult(FailAll
            ? Result.Failure(new Error("Email.SendFailed", "SMTP unavailable."))
            : Result.Success());
    }
}
