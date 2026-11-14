using Application.Features.Users.Services;
using Domain.Users;
using Domain.Users.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactorLogin
{
    public sealed class VerifyTwoFactorLoginCommandHandler
                : IRequestHandler<VerifyTwoFactorLoginCommand, Result<VerifyTwoFactorLoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITotpService _totpService;
        private readonly ITokenService _tokenService;
        private readonly IIdentityService _identityService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VerifyTwoFactorLoginCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITotpService totpService,
            ITokenService tokenService,
            IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _totpService = totpService;
            _tokenService = tokenService;
            _identityService = identityService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<VerifyTwoFactorLoginResponse>> Handle(
            VerifyTwoFactorLoginCommand command,
            CancellationToken ct)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var user = await _userRepository.GetByIdAsync(command.UserId, ct);
            if (user is null)
                return Result<VerifyTwoFactorLoginResponse>.Failure(UserErrors.NotFound);

            // The account must be active / unlocked / not deleted before
            // a 2FA-only login completes.
            var availabilityResult = user.CheckAvailability();
            if (availabilityResult.IsFailure)
                return Result<VerifyTwoFactorLoginResponse>.Failure(availabilityResult.Error);

            // 2FA must actually be enabled for this flow to be reachable.
            if (!user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                return Result<VerifyTwoFactorLoginResponse>.Failure(UserErrors.SecurityErrors.TwoFactorNotEnabled);

            if (!_totpService.Verify(user.TwoFactorSecret, command.TotpCode))
            {
                user.RecordFailedLogin(ipAddress);
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync(ct);

                await _auditService.LogAsync(new AuditEntry(
                    ActorId: user.Id,
                    Action: AuditActions.UserLoginFailed,
                    EntityType: nameof(User),
                    EntityId: user.Id,
                    IpAddress: ipAddress,
                    Succeeded: false,
                    FailureReason: "Invalid TOTP code provided."), ct);

                return Result<VerifyTwoFactorLoginResponse>.Failure(UserErrors.SecurityErrors.InvalidTotpCode);
            }

            var loginResult = user.RecordSuccessfulLogin(ipAddress);
            if (loginResult.IsFailure)
                return Result<VerifyTwoFactorLoginResponse>.Failure(loginResult.Error);

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(ct);

            var claims = new TokenClaims(
                DomainUserId: user.Id,
                UserId: user.IdentityId,
                Email: user.Email.Value,
                Username: user.Username.Value,
                Role: user.Role.ToString());

            var token = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // 2FA logins get the same refresh-token treatment as normal logins.
            await _identityService.UpdateRefreshTokenAsync(user.IdentityId, refreshToken, ct);

            await _auditService.LogAsync(new AuditEntry(
                ActorId: user.Id,
                Action: AuditActions.UserLogin,
                EntityType: nameof(User),
                EntityId: user.Id,
                IpAddress: ipAddress,
                Succeeded: true), ct);

            return Result<VerifyTwoFactorLoginResponse>.Success(
                new VerifyTwoFactorLoginResponse(token, refreshToken, user.Id));
        }
    }
}