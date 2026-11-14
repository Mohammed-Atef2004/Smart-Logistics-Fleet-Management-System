using Application.Features.Users.Commands.Authentication.VerifyTwoFactor;
using Application.Common;
using Application.Features.Users.Services;
using Application.Common.Settings;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;

namespace Application.Features.Users.Commands.Authentication.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly ITotpService _totpService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly ApiSettings _apiSettings;

        public LoginCommandHandler(
          IUserRepository userRepository,
          IUnitOfWork unitOfWork,
          IIdentityService identityService,
          ITokenService tokenService,
          ITotpService totpService,
          IEmailService emailService,
          IOptions<ApiSettings> apiOptions,
          IHttpContextAccessor httpContextAccessor,
          IAuditService auditService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _tokenService = tokenService;
            _totpService = totpService;
            _auditService = auditService;
            _emailService = emailService;
            _apiSettings = apiOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
                return Result<LoginResponse>.Failure(emailResult.Error);

            var user = await _userRepository.GetByEmailAsync(emailResult.Value, ct);

            // Do not reveal whether the email exists (anti-enumeration):
            // unknown email and wrong password return the same error.
            if (user is null)
            {
                // Burn equivalent CPU time so response timing does not
                // differentiate between "email not found" and "wrong password".
                await _identityService.CheckPasswordAsync(command.Email, command.Password, ct);
                return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
            }

            var availabilityResult = user.CheckAvailability();
            if (availabilityResult.IsFailure)
                return Result<LoginResponse>.Failure(availabilityResult.Error);

            // Verify the email is confirmed
            if (!user.IsEmailConfirmed)
            {
                var confirmToken = await _identityService.GenerateEmailConfirmationTokenAsync(user.IdentityId);
                byte[] confirmTokenBytes = Encoding.UTF8.GetBytes(confirmToken);
                string safeconfirmToken = Base64Url.Encode(confirmTokenBytes);

                var confirmationLink = $"{_apiSettings.BaseUrl}/api/authentication/confirm-email?" +
                    $"userId={user.Id}&" +
                    $"token={safeconfirmToken}";

                await _emailService.SendActivationReminderEmailAsync(
                    user.Email.Value,
                    user.FullName.DisplayName,
                    confirmationLink);

                return Result<LoginResponse>.Failure(UserErrors.EmailNotConfirmed);
            }

            var passwordValid = await _identityService.CheckPasswordAsync(command.Email, command.Password, ct);

            if (!passwordValid)
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
                  FailureReason: "Invalid password attempt."), ct);

                return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
            }

            // Two-factor login is not complete yet: do NOT record a successful
            // login here. The VerifyTwoFactorLogin step records it and issues
            // the tokens after a valid TOTP code.
            if (user.IsTwoFactorEnabled)
            {
                return Result<LoginResponse>.Success(new LoginResponse(
                  Token: null,
                  RefreshToken: null,
                  UserId: user.Id,
                  RequiresTwoFactor: true
                ));
            }

            var loginResult = user.RecordSuccessfulLogin(ipAddress);
            if (loginResult.IsFailure)
                return Result<LoginResponse>.Failure(loginResult.Error);

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(ct);


            var claims = new TokenClaims(
                UserId: user.IdentityId,   // string - goes into the sub claim
                DomainUserId: user.Id,     // Guid  - goes into the domain_user_id claim
                Email: user.Email.Value,
                Username: user.Username.Value,
                Role: user.Role.ToString());

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _identityService.UpdateRefreshTokenAsync(user.IdentityId, refreshToken, ct);

            await _auditService.LogAsync(new AuditEntry(
              ActorId: user.Id,
              Action: AuditActions.UserLogin,
              EntityType: nameof(User),
              EntityId: user.Id,
              IpAddress: ipAddress,
              Succeeded: true), ct);

            return Result<LoginResponse>.Success(new LoginResponse(
              Token: accessToken,
              RefreshToken: refreshToken,
              UserId: user.Id,
              RequiresTwoFactor: false));
        }
    }
}