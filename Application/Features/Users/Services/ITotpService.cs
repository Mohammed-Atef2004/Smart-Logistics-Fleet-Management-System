namespace Application.Features.Users.Services
{
    public interface ITotpService
    {
       
        string GenerateSecret();

        string GenerateQrCodeUri(string email, string secret);

        bool Verify(string secret, string code);
    }
}