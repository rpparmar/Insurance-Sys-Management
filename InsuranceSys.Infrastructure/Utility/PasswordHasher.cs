using System.Security.Cryptography;

namespace InsuranceSys.Infrastructure.Utility
{
    public static class PasswordHasher
    {
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;
        private const int Iterations = 100_000;

        public static (string Hash, string Salt) HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            byte[] hash = DeriveKey(password, salt);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            byte[] expectedHash = Convert.FromBase64String(storedHash);
            byte[] actualHash = DeriveKey(password, salt);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using var kdf = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            return kdf.GetBytes(HashSizeBytes);
        }
    }
}
