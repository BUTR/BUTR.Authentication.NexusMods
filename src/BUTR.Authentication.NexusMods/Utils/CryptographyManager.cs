using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace BUTR.Authentication.NexusMods.Utils
{
    /// <summary>
    /// Provides AES GCM
    /// </summary>
    internal static class CryptographyManager
    {
        public static string Encrypt(string plain, string key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plain).AsSpan();
            var keyBytes = Encoding.UTF8.GetBytes(key).AsSpan();

            var nonceSize = AesGcm.NonceByteSizes.MaxSize;
            var tagSize = AesGcm.TagByteSizes.MaxSize;
            var cipherSize = plainBytes.Length;

            var encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            var encryptedData = encryptedDataLength < 1024
                ? stackalloc byte[encryptedDataLength]
                : new byte[encryptedDataLength].AsSpan();

            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            RandomNumberGenerator.Fill(nonce);

            using var aes = new AesGcm(keyBytes);
            aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

            return Convert.ToBase64String(encryptedData);
        }

        public static string? Decrypt(string encrypted, string key)
        {
            var encryptedBytes = Convert.FromBase64String(encrypted).AsSpan();
            var keyBytes = Encoding.UTF8.GetBytes(key).AsSpan();

            var nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedBytes.Slice(0, 4));
            var tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedBytes.Slice(4 + nonceSize, 4));
            var cipherSize = encryptedBytes.Length - 4 - nonceSize - 4 - tagSize;

            var nonce = encryptedBytes.Slice(4, nonceSize);
            var tag = encryptedBytes.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedBytes.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            var plainBytes = cipherSize < 1024
                ? stackalloc byte[cipherSize]
                : new byte[cipherSize];
            using var aes = new AesGcm(keyBytes);
            try { aes.Decrypt(nonce, cipherBytes, tag, plainBytes); }
            catch (Exception) { return null; }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}