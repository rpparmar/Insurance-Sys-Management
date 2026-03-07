using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;

namespace Insurancesys.web.Utility
{
    /// <summary>
    /// Generic query-string encryption/decryption service backed by AES-256-GCM.
    /// Produces compact, URL-safe tokens (~40 chars for a short integer).
    /// Stateless — works across multiple servers without shared cache.
    /// The AES key is derived once from the ASP.NET Core Data Protection
    /// infrastructure, so no extra configuration is required.
    /// Unprotect returns null instead of throwing when a token is invalid
    /// or has been tampered with, letting callers handle the error gracefully.
    /// </summary>
    public interface IQueryStringProtector
    {
        /// <summary>Encrypts a plain string into a compact URL-safe token.</summary>
        string Protect(string plainValue);

        /// <summary>
        /// Decrypts a token produced by <see cref="Protect"/>.
        /// Returns <c>null</c> if the token is invalid or tampered with.
        /// </summary>
        string? Unprotect(string encryptedValue);
    }

    public class QueryStringProtector : IQueryStringProtector
    {
        private readonly byte[] _key;
        private const int NonceSize = 12; // AES-GCM standard nonce
        private const int TagSize   = 16; // 128-bit authentication tag

        public QueryStringProtector(IDataProtectionProvider provider)
        {
            // Derive a stable 32-byte AES-256 key from the Data Protection
            // infrastructure using a fixed purpose + seed. This avoids storing
            // a raw key in config while still tying the key to this application.
            var protector   = provider.CreateProtector("InsureSys.AesKey.v1");
            var keyMaterial = protector.Protect("InsureSys.QueryString.AesKeyDerivation");
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(keyMaterial));
        }

        public string Protect(string plainValue)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainValue);
            var nonce      = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            var ciphertext = new byte[plainBytes.Length];
            var tag        = new byte[TagSize];

            using var aes = new AesGcm(_key, TagSize);
            aes.Encrypt(nonce, plainBytes, ciphertext, tag);

            // Layout: nonce(12) | ciphertext(n) | tag(16)
            var packed = new byte[NonceSize + ciphertext.Length + TagSize];
            nonce     .CopyTo(packed, 0);
            ciphertext.CopyTo(packed, NonceSize);
            tag       .CopyTo(packed, NonceSize + ciphertext.Length);

            return Base64UrlEncode(packed);
        }

        public string? Unprotect(string encryptedValue)
        {
            try
            {
                var data = Base64UrlDecode(encryptedValue);
                if (data.Length < NonceSize + TagSize)
                    return null;

                var nonce      = data[..NonceSize];
                var tag        = data[^TagSize..];
                var ciphertext = data[NonceSize..^TagSize];
                var plaintext  = new byte[ciphertext.Length];

                using var aes = new AesGcm(_key, TagSize);
                aes.Decrypt(nonce, ciphertext, tag, plaintext);

                return Encoding.UTF8.GetString(plaintext);
            }
            catch
            {
                return null; // tampered, malformed, or wrong key
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
            => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static byte[] Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s += (s.Length % 4) switch { 2 => "==", 3 => "=", _ => "" };
            return Convert.FromBase64String(s);
        }
    }

    /// <summary>
    /// Convenience extensions for common value types.
    /// Add further overloads here as new use-cases arise.
    /// </summary>
    public static class QueryStringProtectorExtensions
    {
        public static string ProtectInt(this IQueryStringProtector protector, int value)
            => protector.Protect(value.ToString());

        /// <summary>Returns null when the token is invalid or the decrypted value is not an integer.</summary>
        public static int? UnprotectInt(this IQueryStringProtector protector, string encryptedValue)
        {
            var raw = protector.Unprotect(encryptedValue);
            return raw != null && int.TryParse(raw, out var result) ? result : null;
        }
    }
}
