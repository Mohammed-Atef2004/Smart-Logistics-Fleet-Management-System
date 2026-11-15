using Application.Features.Users.Commands.Authentication.ConfirmEmail;
using Application.Features.Users.Commands.Authentication.ConfirmEmailChange;
using Application.Features.Users.Commands.Authentication.ForgotPassword;
using Application.Features.Users.Commands.Authentication.Login;
using Application.Features.Users.Commands.Authentication.Logout;
using Application.Features.Users.Commands.Authentication.Register;
using Application.Features.Users.Commands.Authentication.ResetPassword;
using Application.Features.Users.Commands.Authentication.VerifyTwoFactor;
using Application.Features.Users.Commands.Authentication.VerifyTwoFactorLogin;
using Domain.Users.Errors;
using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Read "domain_user_id" from the token instead of sub
        // because sub holds the IdentityId (string), not the domain Guid
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue("domain_user_id")
            ?? throw new UnauthorizedAccessException("User ID not found in claims"));

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ─────────────────────────────────────────────
        // Authentication
        // ─────────────────────────────────────────────

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result.Error);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result);
        }

        // This endpoint is AllowAnonymous because the user is not logged in yet
        // so the UserId is sent in the body instead of read from the token
        [HttpPost("verify-2fa-login")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyTwoFactorLogin(
            [FromBody] VerifyTwoFactorLoginRequest request,
            CancellationToken ct)
        {
            var result = await _mediator.Send(
                new VerifyTwoFactorLoginCommand(request.UserId, request.TotpCode), ct);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken ct)
        {
            if (!Base64Url.TryDecode(token, out var decodedBytes))
                return BadRequest(new Domain.SharedKernel.Error("User.Token.Invalid", "The confirmation token is malformed."));
            string originalToken = Encoding.UTF8.GetString(decodedBytes);

            var command = new ConfirmEmailCommand(userId, originalToken);
            var result = await _mediator.Send(command, ct);
            if (result.IsSuccess)
                return Ok("Email confirmed successfully! You can now login.");

            return BadRequest(result);
        }

        [HttpGet("confirm-email-change")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange(
            [FromQuery] Guid userId,
            [FromQuery] string newEmail,
            [FromQuery] string token,
            CancellationToken ct)
        {
            if (!Base64Url.TryDecode(token, out var decodedBytes))
                return BadRequest(new Domain.SharedKernel.Error("User.Token.Invalid", "The confirmation token is malformed."));
            string originalToken = Encoding.UTF8.GetString(decodedBytes);

            var result = await _mediator.Send(new ConfirmEmailChangeCommand(userId, newEmail, originalToken), ct);

            if (result.IsSuccess)
                return Ok("Your email has been updated successfully.");

            return BadRequest(result.Error);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
                return Ok("If your email exists, a reset link has been sent.");

            return BadRequest(result.Error);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
        {
            if (!Base64Url.TryDecode(command.Token, out var decodedBytes))
                return BadRequest(new Domain.SharedKernel.Error("User.Token.Invalid", "The reset token is malformed."));
            string originalToken = Encoding.UTF8.GetString(decodedBytes);

            var updatedCommand = command with { Token = originalToken };
            var result = await _mediator.Send(updatedCommand, ct);
            if (result.IsSuccess)
                return Ok("Password Reset Success");
            return BadRequest(result);
        }
    }

    public sealed record VerifyTwoFactorLoginRequest(
        Guid UserId,
        string TotpCode);
}