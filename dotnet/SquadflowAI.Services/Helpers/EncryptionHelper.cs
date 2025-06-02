using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Helpers
{
    public static class EncryptionHelper
    {
        public static string EncryptApiKey(string apiKey, IConfiguration configuration)
        {
            var getKey = configuration.GetValue<string>("SECRET_KEY_32");
            var getIV = configuration.GetValue<string>("SECRET_KEY_16");
            byte[] key = Convert.FromBase64String(getKey);
            byte[] iv = Convert.FromBase64String(getIV);
            byte[] Key = key;
            byte[] IV = iv;

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(apiKey);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DecryptApiKey(string encryptedApiKey, IConfiguration configuration)
        {
            var getKey = configuration.GetValue<string>("SECRET_KEY_32");
            var getIV = configuration.GetValue<string>("SECRET_KEY_16");
            byte[] key = Convert.FromBase64String(getKey);
            byte[] iv = Convert.FromBase64String(getIV);
            byte[] Key = key;
            byte[] IV = iv;

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(Convert.FromBase64String(encryptedApiKey)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }



    }
}
