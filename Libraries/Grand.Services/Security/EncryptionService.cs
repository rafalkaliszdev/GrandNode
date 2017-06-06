﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Grand.Core.Domain.Security;
using System.Security.Cryptography;

namespace Grand.Services.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly SecuritySettings _securitySettings;
        public EncryptionService(SecuritySettings securitySettings)
        {
            this._securitySettings = securitySettings;
        }

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public virtual string CreateSaltKey(int size)
        {
            //so RNG isnt available in core, use RSA
            //System.Security.Cryptography.serv
            //tbh


            //https://stackoverflow.com/questions/38632735/rngcryptoserviceprovider-in-net-core
            //RandomNumberGenerator.Create() is the only way to get an RNG instance on .NET Core, 
            //and since it works on both .NET Core and .NET Framework is the most portable.





            //Generate a cryptographic random number
            var rng = RandomNumberGenerator.Create();//new
            //var rng = new RNGCryptoServiceProvider();//previous
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1")
        {
            if (String.IsNullOrEmpty(passwordFormat))
                passwordFormat = "SHA1";
            string saltAndPassword = String.Concat(password, saltkey);

            HashAlgorithm algorithm = SHA1.Create();

            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        public virtual string EncryptText(string plainText, string encryptionPrivateKey = "")
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (String.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;
            //tbh
            //var tDESalg = new TripleDESCryptoServiceProvider();
            //tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            //tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

            //byte[] encryptedBinary = EncryptTextToMemory(plainText, tDESalg.Key, tDESalg.IV);
            //return Convert.ToBase64String(encryptedBinary);

            return "";
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        public virtual string DecryptText(string cipherText, string encryptionPrivateKey = "")
        {
            if (String.IsNullOrEmpty(cipherText))
                return cipherText;

            if (String.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            //tbh

            //var tDESalg = new TripleDESCryptoServiceProvider();
            //tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            //tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

            //byte[] buffer = Convert.FromBase64String(cipherText);
            //return DecryptTextFromMemory(buffer, tDESalg.Key, tDESalg.IV);

            return "";
        }

        #region Utilities

        private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            //using (var ms = new MemoryStream()) {
            //    using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write)) {
            //        byte[] toEncrypt = new UnicodeEncoding().GetBytes(data);
            //        cs.Write(toEncrypt, 0, toEncrypt.Length);
            //        cs.FlushFinalBlock();
            //    }

            //    return ms.ToArray();
            //}
            return default(byte[]);
        }

        private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            //tbh
            //using (var ms = new MemoryStream(data)) {
            //    using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
            //    {
            //        var sr = new StreamReader(cs, new UnicodeEncoding());
            //        return sr.ReadLine();
            //    }
            //}
            return default(string);

        }

        #endregion
    }
}
