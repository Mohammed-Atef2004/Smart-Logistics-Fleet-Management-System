namespace Application.Features.Users.Commands.Authentication.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;
}