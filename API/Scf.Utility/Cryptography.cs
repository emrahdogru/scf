using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Scf.Utility
{

    public static class Cryptography
    {
 
        public enum CryptType
        {
            Encrypt,
            Decrypt
        }

        internal static string? Crypt(string cryptData, string cryptPwd, CryptType cryptType)
        {
            if (cryptData == null)
                throw new ArgumentNullException(nameof(cryptData));

            byte[] keySalt = new byte[] { 0x26, 0x19, 0x81, 0x4E, 0xA0, 0x6D, 0x95, 0x34, 0x26, 0x75, 0x64, 0x05, 0xF6 };
            PasswordDeriveBytes pwdBytes = new(cryptPwd, keySalt);

            Aes crypAlg = Aes.Create();
            crypAlg.Key = pwdBytes.GetBytes(32);
            crypAlg.IV = pwdBytes.GetBytes(16);

            try
            {
                using ICryptoTransform cryptoTransform = (cryptType == CryptType.Encrypt)
                    ? crypAlg.CreateEncryptor()
                    : crypAlg.CreateDecryptor();
                using MemoryStream memStream = new();
                using CryptoStream cryptStream = new(memStream, cryptoTransform, CryptoStreamMode.Write);
                byte[]? cryptBytes = null;
                switch (cryptType)
                {
                    case CryptType.Encrypt:
                        cryptBytes = Encoding.Unicode.GetBytes(cryptData);
                        break;
                    case CryptType.Decrypt:
                        cryptBytes = Convert.FromBase64String(cryptData?.Replace(" ", "+") ?? "");
                        break;
                }

                if (cryptBytes != null && cryptBytes.Length > 0)
                {
                    cryptStream.Write(cryptBytes, 0, cryptBytes.Length);
                    cryptStream.FlushFinalBlock();
                    switch (cryptType)
                    {
                        case CryptType.Encrypt: return Convert.ToBase64String(memStream.ToArray());
                        case CryptType.Decrypt: return Encoding.Unicode.GetString(memStream.ToArray());
                    }
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return null;
            }

            return null;
        }

        private static string? CryptForField(string cryptData, CryptType cryptType)
        {
            return Crypt(cryptData, "~Ü;♣\"46☼↑36☼5", cryptType);
        }

        public static string? EncryptData(string data)
        {
            if (data == null)
                return null;

            return CryptForField(data, CryptType.Encrypt);
        }

        public static string? DecryptData(string data)
        {
            if (data == null)
                return null;

            return CryptForField(data, CryptType.Decrypt);
        }

    }
}
