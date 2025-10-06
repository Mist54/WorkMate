using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WorkMate.Helpers
{
    public class Encryptor
    {
        private static readonly byte[] key = GetAesKey();
        private static readonly byte[] iv = GetAesIV();

        private static byte[] GetAesKey()
        {
            string aesKey = Environment.GetEnvironmentVariable("AES_KEY");
            if (string.IsNullOrEmpty(aesKey))
            {
                aesKey = ConfigurationManager.AppSettings["AesKey"]; // Fallback to Web.config
            }

            if (string.IsNullOrEmpty(aesKey))
            {
                throw new InvalidOperationException("AES_KEY environment variable or Web.config setting is not set. Please set it to a 32-character key.");
            }
            return Encoding.UTF8.GetBytes(aesKey);
        }

        private static byte[] GetAesIV()
        {
            string aesIV = Environment.GetEnvironmentVariable("AES_IV");
            if (string.IsNullOrEmpty(aesIV))
            {
                aesIV = ConfigurationManager.AppSettings["AesIV"]; // Fallback to Web.config
            }

            if (string.IsNullOrEmpty(aesIV))
            {
                throw new InvalidOperationException("AES_IV environment variable or Web.config setting is not set. Please set it to a 16-character IV.");
            }
            return Encoding.UTF8.GetBytes(aesIV);
        }

        
        public static string EncryptUrlSafe(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    var base64 = Convert.ToBase64String(ms.ToArray());

                    // Make URL-safe
                    return base64.Replace("+", "-")
                                 .Replace("/", "_")
                                 .Replace("=", "");
                }
            }
        }

        public static string DecryptUrlSafe(string cipherText)
        {
            // Restore to normal Base64
            string base64 = cipherText.Replace("-", "+")
                                      .Replace("_", "/");

            // Pad if needed
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var buffer = Convert.FromBase64String(base64);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}