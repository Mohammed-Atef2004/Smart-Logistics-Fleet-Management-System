using Application.Features.Users.Services;
using Domain.Users;
using Domain.Users.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Commands.Authentication.ResetPassword
{
    public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _auditService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITotpService _totpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResetPasswordCommandHandler(
          IUserRepository userRepository,
          IIdentityService identityService,
          IPasswordHasher passwordHasher,
          ITokenService tokenService,
          IAuditService auditService,
          IUnitOfWork unitOfWork,
          ITotpService totpService,
          IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _auditService = auditService;
            _totpService = totpService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken ct)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user is null)
                return Result.Failure(UserErrors.NotFound);

            var isPasswordReused = user.PasswordHashes.Any(h => _passwordHasher.VerifyPassword(command.NewPassword, h));

            var identityResult = await _identityService.ResetPasswordAsync(
              user.IdentityId,
              command.Token,
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
                await _unitOfWork.CompleteAsync(ct);
                return Result.Failure(identityResult.Error);
            }

            // RecordPasswordChange rejects reused passwords, updates the
            // history, sets PasswordChangedAt and raises the domain event.
            var newHash = _passwordHasher.HashPassword(command.NewPassword);
            var recordResult = user.RecordPasswordChange(newHash, isPasswordReused);
            if (recordResult.IsFailure)
                return recordResult;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(ct);

            // A password reset must invalidate any existing session: the old
            // refresh token can no longer mint access tokens.
            await _tokenService.RevokeRefreshTokenAsync(user.IdentityId, ct);

            await _auditService.LogAsync(new AuditEntry(
              ActorId: user.Id,
              Action: AuditActions.UserResetPassword,
              EntityType: "User",
              EntityId: user.Id,
              IpAddress: ipAddress,
              Succeeded: true
            ), ct);
            return Result.Success();
        }
    }
}