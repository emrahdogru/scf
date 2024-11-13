using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Utility
{
    public class Tools
    {

        /// <summary>
        /// MD5 şifreleme
        /// </summary>
        /// <param name="input">Şifrenelecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public static byte[] Md5AsByteArray(byte[] input)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(input);
            return data;
        }

        /// <summary>
        /// MD5 şifreleme
        /// </summary>
        /// <param name="input">Şifrenelecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public static string Md5(byte[] input)
        {
            if (input == null) return string.Empty;
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(input);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// MD5 şifreleme
        /// </summary>
        /// <param name="input">Şifrenelecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public static string Md5(System.IO.Stream input)
        {
            if (input == null) return string.Empty;
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(input);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// MD5 şifreleme
        /// </summary>
        /// <param name="input">Şifrenelecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public static string Md5(string input)
        {
            return Md5(Encoding.Default.GetBytes(input));
        }

        /// <summary>
        /// SHA256 şifreleme
        /// </summary>
        /// <param name="input">Şifrenelecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public static string Sha256(byte[] input)
        {
            if (input == null) return string.Empty;

            var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] data = sha256.ComputeHash(input);
            StringBuilder stb = new();
            for (int i = 0; i < data.Length; i++)
            {
                stb.Append(data[i].ToString("x2"));
            }
            return stb.ToString();
        }

        /// <summary>
        /// SHA256 şifreleme
        /// </summary>
        /// <param name="input">Şifrenelecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public static string Sha256(string input)
        {
            return Sha256(Encoding.UTF8.GetBytes(input));
        }
    }
}
