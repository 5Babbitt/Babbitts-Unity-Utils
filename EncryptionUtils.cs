using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BabbittsUnityUtils
{
    public static class EncryptionUtils
    {
        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKey();
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            aes.IV.CopyTo(result, 0);
            encryptedBytes.CopyTo(result, aes.IV.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            
            var fullBytes = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = DeriveKey();

            var iv = new byte[aes.BlockSize / 8];
            var encrypted = new byte[fullBytes.Length - iv.Length];
            Array.Copy(fullBytes, 0, iv, 0, iv.Length);
            Array.Copy(fullBytes, iv.Length, encrypted, 0, encrypted.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static byte[] DeriveKey()
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier));
        }
    }
}