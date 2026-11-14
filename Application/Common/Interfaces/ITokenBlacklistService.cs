namespace Application.Common.Interfaces;

public interface ITokenBlacklistService
{
    void Revoke(string jti, DateTime tokenExpiry);
    bool IsRevoked(string jti);
}