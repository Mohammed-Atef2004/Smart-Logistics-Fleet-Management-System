using Domain.Users.Interfaces.Services;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithm, KeySize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool Verify(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split('-');
            var hash = Convert.FromHexString(parts[0]);
            var salt = Convert.FromHexString(parts[1]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithm, KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}