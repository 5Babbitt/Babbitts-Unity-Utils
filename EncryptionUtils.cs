using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BabbittsUnityUtils
{
    public static class EncryptionUtils
    {
        private const string SecretFileName = "AppSecret.bytes";
        private const string SaltFileName = "Salt.bytes";
        private static readonly string SecretFolderPath = Path.Combine(Application.dataPath, "_Project", "Resources", "Secrets");
        private static readonly string SecretFilePath = Path.Combine(SecretFolderPath, SecretFileName);
        private static readonly string SaltFilePath = Path.Combine(SecretFolderPath, SaltFileName);

        public static string Encrypt(string plainText)
        {
            return Encrypt(plainText, LoadOrGenerateSecret());
        }

        public static string Decrypt(string cipherText)
        {
            return Decrypt(cipherText, LoadOrGenerateSecret());
        }

        public static string Encrypt(string plainText, string appSecret)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKey(appSecret);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            aes.IV.CopyTo(result, 0);
            encryptedBytes.CopyTo(result, aes.IV.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText, string appSecret)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            var fullBytes = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = DeriveKey(appSecret);

            var iv = new byte[aes.BlockSize / 8];
            var encrypted = new byte[fullBytes.Length - iv.Length];
            Array.Copy(fullBytes, 0, iv, 0, iv.Length);
            Array.Copy(fullBytes, iv.Length, encrypted, 0, encrypted.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Loads the app secret from disk. Generates and saves a new one if it doesn't exist.
        /// Store the generated file's folder in .gitignore to keep it out of source control.
        /// </summary>
        public static string LoadOrGenerateSecret()
        {
            if (File.Exists(SecretFilePath))
                return File.ReadAllText(SecretFilePath, Encoding.UTF8);

            return GenerateAndSaveSecret();
        }

        private static string GenerateAndSaveSecret()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var secret = Convert.ToBase64String(randomBytes);

            EnsureSecretsFolder();
            File.WriteAllText(SecretFilePath, secret, Encoding.UTF8);

            Debug.Log($"[EncryptionUtils] Generated new app secret at {SecretFilePath}. Add this folder to .gitignore.");

            return secret;
        }

        private static byte[] LoadOrGenerateSalt()
        {
            if (File.Exists(SaltFilePath))
                return Convert.FromBase64String(File.ReadAllText(SaltFilePath, Encoding.UTF8));

            return GenerateAndSaveSalt();
        }

        private static byte[] GenerateAndSaveSalt()
        {
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            EnsureSecretsFolder();
            File.WriteAllText(SaltFilePath, Convert.ToBase64String(salt), Encoding.UTF8);

            Debug.Log($"[EncryptionUtils] Generated new salt at {SaltFilePath}. Add this folder to .gitignore.");

            return salt;
        }

        private static void EnsureSecretsFolder()
        {
            if (!Directory.Exists(SecretFolderPath))
                Directory.CreateDirectory(SecretFolderPath);
        }

        private static byte[] DeriveKey(string appSecret)
        {
            string machineSecret = SystemInfo.deviceUniqueIdentifier;
            string combinedPassword = appSecret + machineSecret;

            using var pbkdf2 = new Rfc2898DeriveBytes(combinedPassword, LoadOrGenerateSalt(), 10000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
    }
}