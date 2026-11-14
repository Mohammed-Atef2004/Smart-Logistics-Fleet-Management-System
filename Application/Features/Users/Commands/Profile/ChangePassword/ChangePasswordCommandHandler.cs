using Application.Features.Users.Commands.Authentication.VerifyTwoFactor;
using Application.Features.Users.Services;
using Domain.Users;
using Domain.Users.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Commands.Profile.ChangePassword
{
    public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly ITotpService _totpService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher _passwordHasher;


        public ChangePasswordCommandHandler(
            IUserRepository userRepository,
            IIdentityService identityService,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            ITotpService totpService,
            IUnitOfWork unitOfWork,
            IAuditService auditService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _tokenService = tokenService;
            _totpService = totpService;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken ct)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            var availabilityResult = user.CheckAvailability();
            if (availabilityResult.IsFailure)
                return availabilityResult;

            var isPasswordValid = await _identityService.CheckPasswordAsync(user.Email.Value, command.CurrentPassword, ct);
            if (!isPasswordValid)
            {
                user.RecordFailedLogin(ipAddress);
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync(ct);
                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserChangePassword,
                    EntityType: nameof(User),
                    EntityId: user.Id,
                    IpAddress: ipAddress,
                    Succeeded: false,
                    FailureReason: "Invalid current password."), ct);
                return Result.Failure(UserErrors.InvalidCredentials);
            }

            if (user.IsTwoFactorEnabled)
            {
                if (string.IsNullOrEmpty(command.TwoFactorCode) || !_totpService.Verify(user.TwoFactorSecret!, command.TwoFactorCode))
                {
                    await _auditService.LogAsync(new AuditEntry(
                        ActorId: user.Id,
                        Action: AuditActions.UserChangePassword,
                        EntityType: nameof(User),
                        EntityId: user.Id,
                        IpAddress: ipAddress,
                        Succeeded: false,
                        FailureReason: "Invalid 2FA code during password change."), ct);
                    return Result.Failure(UserErrors.SecurityErrors.InvalidTotpCode);
                }
            }

            var isPasswordReused = user.PasswordHashes.Any(h => _passwordHasher.VerifyPassword(command.NewPassword, h));
            var identityResult = await _identityService.ChangePasswordAsync(
                user.IdentityId,command.CurrentPassword,
                command.NewPassword,
                ct);

            if (identityResult.IsFailure)
            {
                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserResetPassword,
                    EntityType: "User",
                    EntityId: user.Id,
                    IpAddress: ipAddress,
                    Succeeded: false,
                    FailureReason: identityResult.Error.Message
                ), ct);
                return Result.Failure(identityResult.Error);
            }

            var newHash = _passwordHasher.HashPassword(command.NewPassword);

            // RecordPasswordChange rejects reused passwords, updates the
            // history, sets PasswordChangedAt and raises the domain event.
            var recordResult = user.RecordPasswordChange(newHash, isPasswordReused);
            if (recordResult.IsFailure)
                return recordResult;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(ct);

            // A successful password change invalidates the current session's
            // refresh token; any stolen refresh token stops working.
            await _tokenService.RevokeRefreshTokenAsync(user.IdentityId, ct);

            await _auditService.LogAsync(new AuditEntry(
                ActorId: user.Id,
                Action: AuditActions.UserChangePassword,
                EntityType: nameof(User),
                EntityId: user.Id,
                IpAddress: ipAddress,
                Succeeded: true), ct);

            return Result.Success();
        }
    }
}