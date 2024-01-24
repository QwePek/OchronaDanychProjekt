using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using WebApp.Shared.Model;

namespace WebApp.Shared
{
    public class AESUtils
    {
        public static string? KEY { private get; set; }
        public static string? IV { private get; set; }

        public static string Encrypt(string text)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(KEY);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(text);

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipher)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(KEY);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipher)))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (var srDecrypt = new StreamReader(csDecrypt))
                            return srDecrypt.ReadToEnd();
            }
        }

		public static string hashPassword(PasswordHasher<User> hasher, User user, string password, string salt)
		{
			password += salt;
			return hasher.HashPassword(user, password);
		}

		public static PasswordVerificationResult verifyPassword(PasswordHasher<User> hasher, User user, string providedPassword, string salt, string hashedPassword)
		{
			providedPassword += salt;
			return hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
		}

		public static string generateSalt(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
		}
	}
}
