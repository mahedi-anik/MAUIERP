using MAUIERP.ApplicationLayer.Common.Interfaces;
using System.Security.Cryptography;

namespace MAUIERP.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var parts = passwordHash.Split('-');
            if (parts.Length != 2) return false;

            var hash = Convert.FromHexString(parts[0]);
            var salt = Convert.FromHexString(parts[1]);

            var newHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, hash.Length);

            return CryptographicOperations.FixedTimeEquals(newHash, hash);
        }
    }
}
