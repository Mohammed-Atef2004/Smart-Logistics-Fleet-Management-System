using Application.Features.Users.Services;
using Domain.Users;
using Domain.Users.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Commands.Security.Confirm2FASetup
{
    public sealed class Confirm2FASetupCommandHandler
            : IRequestHandler<Confirm2FASetupCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITotpService _totpService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Confirm2FASetupCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITotpService totpService,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _totpService = totpService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(Confirm2FASetupCommand command, CancellationToken ct)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var user = await _userRepository.GetByIdAsync(command.UserId);

            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            // Locked / deactivated / deleted accounts cannot enable 2FA.
            var availabilityResult = user.CheckAvailability();
            if (availabilityResult.IsFailure)
                return availabilityResult;

            var isVerified = _totpService.Verify(command.NewSecret, command.TotpCode);

            if (!isVerified)
            {
                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.User2FAEnabled,
                    EntityType: "User",
                    EntityId: user.Id,
                    IpAddress: ipAddress,
                    Succeeded: false,
                    FailureReason: "Invalid TOTP code entered during confirmation."
                ), ct);
                return Result.Failure(UserErrors.SecurityErrors.InvalidTotpCode);
            }
            var enableResult = user.EnableTwoFactor(command.NewSecret);
            if (enableResult.IsFailure)
                return enableResult;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(ct);

            await _auditService.LogAsync(new AuditEntry(
                ActorId: user.Id,
                Action: AuditActions.User2FAEnabled,
                EntityType: "User",
                EntityId: user.Id,
                IpAddress: ipAddress,
                Succeeded: true,
                FailureReason: null
            ), ct);
            return Result.Success();
        }
    }
}