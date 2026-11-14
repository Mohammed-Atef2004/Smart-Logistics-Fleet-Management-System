using Application.Features.Users.Commands.Authentication.VerifyTwoFactor;
using Application.Common;
using Application.Features.Users.Services;
using Application.Common.Settings;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace Application.Features.Users.Commands.Authentication.ForgotPassword
{
    public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IIdentityService identityService,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IOptions<ApiSettings> apiOptions,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _apiSettings = apiOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
        }

        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct)
        {
            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
            {
                return Result.Success();
            }
            var user = await _userRepository.GetByEmailAsync(emailResult.Value, ct);
            if (user is null)
            {
                return Result.Success();
            }

            if (!user.IsEmailConfirmed) 
            {
                var confirmToken = await _identityService.GenerateEmailConfirmationTokenAsync(user.IdentityId);
                byte[] confirmTokenBytes = Encoding.UTF8.GetBytes(confirmToken);
                string safeconfirmToken = Base64Url.Encode(confirmTokenBytes);
                var confirmationLink = $"{_apiSettings.BaseUrl}/api/authentication/confirm-email?" +
                    $"userId={user.Id}&" +
                    $"token={safeconfirmToken}";
                var confirmResult =await _emailService.SendActivationReminderEmailAsync
                    (user.Email.Value,
                    user.FullName.DisplayName,
                    confirmationLink);

                string? reason;
                if (confirmResult.IsFailure)
                {
                    reason = $"Email failed: {confirmResult.Error.Message}";
                }
                else
                {
                    reason = "User not confirmed; activation reminder sent.";
                }

                await _auditService.LogAsync(new AuditEntry(
                ActorId: user.Id,
                Action: AuditActions.UserForgotPassword,
                EntityType: "User",
                EntityId: user.Id,
                IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                Succeeded: confirmResult.IsSuccess,
                FailureReason: reason
                ), ct);

                return Result.Success();
            }
            if (user.IsDeleted)
                return Result.Success();

            if (!user.IsActive)
                return Result.Success();

            var token = await _identityService.GeneratePasswordResetTokenAsync(user.Email.Value, ct);

            if (string.IsNullOrEmpty(token))
            {
                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserForgotPassword,
                    EntityType: "User",
                    EntityId: user.Id,
                    IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    Succeeded: false,
                    FailureReason: "Failed to generate identity token."
                ), ct);
                await _unitOfWork.CompleteAsync(ct);
                return Result.Success();
            }

            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            string safeToken = Base64Url.Encode(tokenBytes);
            var resetLink = $"{_apiSettings.BaseUrl}/api/authentication/reset-password?" +
                $"userId={user.Id}&" +
                $"token={safeToken}";

            var emailTask = await _emailService.SendPasswordResetEmailAsync(
                user.Email.Value,
                user.FullName.DisplayName,
                resetLink, 
                ct);

            await _auditService.LogAsync(new AuditEntry(
            ActorId: user.Id,
            Action: AuditActions.UserForgotPassword,
            EntityType: "User",
            EntityId: user.Id,
            IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            Succeeded: emailTask.IsSuccess,
            FailureReason: emailTask.IsFailure ? emailTask.Error.Message : null
            ), ct);
            await _unitOfWork.CompleteAsync(ct);

            if (emailTask.IsFailure)
            {
                return Result.Failure(emailTask.Error);
            }
            return Result.Success();
        }
    }
}