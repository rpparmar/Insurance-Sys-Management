using System.Security.Cryptography;
using System.Text;

namespace InsuranceSys.Infrastructure.Utility
{
    public static class Cryptography
    {
        // Prefer loading from configuration, not literals.
        private static string GetMasterPassword() => "Insyscrypto2026";
        private const int KeySizeBits = 256;
        private const int IvSizeBytes = 16; // AES block size
        private const int SaltSizeBytes = 16;
        private const int Pbkdf2Iterations = 100_000; // tune per your policy

        /// <summary>Standard Base64 output (may contain + and / — avoid in URL path segments).</summary>
        public static string EncryptUtf16(string plainText)
        {
            byte[] packed = EncryptToPacked(plainText);
            return Convert.ToBase64String(packed);
        }

        /// <summary>
        /// Same ciphertext as <see cref="EncryptUtf16"/> but encoded as Base64url (no +, /, or padding issues in URLs).
        /// </summary>
        public static string EncryptUtf16UrlSafe(string plainText)
        {
            byte[] packed = EncryptToPacked(plainText);
            return Base64UrlEncode(packed);
        }

        /// <summary>
        /// Decrypts values produced by <see cref="EncryptUtf16"/> or <see cref="EncryptUtf16UrlSafe"/>
        /// (accepts both standard Base64 and Base64url).
        /// </summary>
        public static string DecryptUtf16(string base64CipherText)
        {
            byte[] packed = DecodeBase64Flexible(base64CipherText);
            ReadOnlySpan<byte> salt = packed.AsSpan(0, SaltSizeBytes);
            ReadOnlySpan<byte> cipher = packed.AsSpan(SaltSizeBytes);
            using var aes = Aes.Create();
            aes.KeySize = KeySizeBits;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using var kdf = new Rfc2898DeriveBytes(
                GetMasterPassword(),
                salt.ToArray(),
                Pbkdf2Iterations,
                HashAlgorithmName.SHA256);
            aes.Key = kdf.GetBytes(aes.KeySize / 8);
            aes.IV = kdf.GetBytes(IvSizeBytes);
            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipher.ToArray(), 0, cipher.Length);
            return Encoding.Unicode.GetString(plainBytes);
        }

        private static byte[] EncryptToPacked(string plainText)
        {
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            using var aes = Aes.Create();
            aes.KeySize = KeySizeBits;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using var kdf = new Rfc2898DeriveBytes(
                GetMasterPassword(),
                salt,
                Pbkdf2Iterations,
                HashAlgorithmName.SHA256);
            aes.Key = kdf.GetBytes(aes.KeySize / 8);
            aes.IV = kdf.GetBytes(IvSizeBytes);
            using var encryptor = aes.CreateEncryptor();
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            // Format: [salt | cipher]
            byte[] packed = new byte[salt.Length + cipherBytes.Length];
            Buffer.BlockCopy(salt, 0, packed, 0, salt.Length);
            Buffer.BlockCopy(cipherBytes, 0, packed, salt.Length, cipherBytes.Length);
            return packed;
        }

        private static string Base64UrlEncode(byte[] bytes)
            => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        /// <summary>
        /// Decodes standard Base64 or Base64url to the same byte array (same underlying binary).
        /// </summary>
        private static byte[] DecodeBase64Flexible(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("Cipher text cannot be null or empty.", nameof(s));

            s = s.Trim();
            s = s.Replace('-', '+').Replace('_', '/');
            s += (s.Length % 4) switch { 2 => "==", 3 => "=", _ => "" };
            return Convert.FromBase64String(s);
        }
    }
}
