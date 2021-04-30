using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarAPI
{
    class RSA
    {
        private RSACryptoServiceProvider rsa;

        /// <summary>
        /// Public key for encryption
        /// </summary>
        public string PublicKey { get; }

        /// <summary>
        /// New RSA encryption/decryption with random generated public/private key
        /// </summary>
        public RSA()
        {
            rsa = new RSACryptoServiceProvider();
            PublicKey = rsa.ToXmlString(false);
        }

        /// <summary>
        /// New RSA encryption/decryption with known public key
        /// </summary>
        /// <param name="publicKey">Public Key to encrypt messages with</param>
        public RSA(string publicKey)
        {
            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            PublicKey = publicKey;
        }

        /// <summary>
        /// Uses public Key to encrypt the Data
        /// </summary>
        /// <param name="data">The Data to encrypt</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] _data)
        {
            using (RSACryptoServiceProvider newRsa = new RSACryptoServiceProvider())
            {
                newRsa.ImportParameters(rsa.ExportParameters(false));
                return newRsa.Encrypt(_data, true);
            }
        }

        /// <summary>
        /// Uses private Key to decrypt the Data
        /// </summary>
        /// <param name="data">The Data to decrypt</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] _data)
        {
            using (RSACryptoServiceProvider newRsa = new RSACryptoServiceProvider())
            {
                newRsa.ImportParameters(rsa.ExportParameters(true));
                return newRsa.Decrypt(_data, true);
            }
        }
    }
}
