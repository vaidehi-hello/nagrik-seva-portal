using System.Security.Cryptography;
using System.Text;

namespace NagrikSevaAPI.Helpers
{
    public static class AesEncryptionHelper
    {
        private const string SECRET_KEY = "NagrikSeva2024!!SecretKey32Chars";

        public static string Encrypt(string plainText)
        {
            try
            {
                var salt = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                    rng.GetBytes(salt);

                var (key, iv) = DeriveKeyAndIV(
                    Encoding.UTF8.GetBytes(SECRET_KEY), salt);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                var plaintextBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(
                    plaintextBytes, 0, plaintextBytes.Length);

                var prefix = Encoding.ASCII.GetBytes("Salted__");
                var result = prefix.Concat(salt).Concat(encryptedBytes).ToArray();
                return Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Encryption failed: {ex.Message}");
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                var salt = encryptedBytes.Skip(8).Take(8).ToArray();
                var ciphertext = encryptedBytes.Skip(16).ToArray();

                var (key, iv) = DeriveKeyAndIV(
                    Encoding.UTF8.GetBytes(SECRET_KEY), salt);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(
                    ciphertext, 0, ciphertext.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Decryption failed: {ex.Message}");
            }
        }

        private static (byte[] key, byte[] iv) DeriveKeyAndIV(
            byte[] password, byte[] salt)
        {
            var derived = new List<byte>();
            byte[] block = Array.Empty<byte>();

            while (derived.Count < 48)
            {
                using var md5 = MD5.Create();
                var input = block.Concat(password).Concat(salt).ToArray();
                block = md5.ComputeHash(input);
                derived.AddRange(block);
            }

            return (derived.Take(32).ToArray(),
                    derived.Skip(32).Take(16).ToArray());
        }
    }
}