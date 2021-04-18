using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InventarServer
{
    class RSA
    {
        private const int keySize = 1024;

        private RSACryptoServiceProvider rsa;

        /// <summary>
        /// Public key for encryption
        /// </summary>
        public byte[] PublicKey { get; }

        /// <summary>
        /// New RSA encryption/decryption with random generated public/private key
        /// </summary>
        public RSA()
        {
            rsa = new RSACryptoServiceProvider(keySize);
            PublicKey = rsa.ExportParameters(false).Modulus;
        }

        /// <summary>
        /// New RSA encryption/decryption with known public key
        /// </summary>
        /// <param name="publicKey">Public Key to encrypt messages with</param>
        public RSA(byte[] publicKey)
        {
            rsa = new RSACryptoServiceProvider(keySize);
            RSAParameters rsaParam = rsa.ExportParameters(false);
            rsaParam.Modulus = publicKey;
            rsa.ImportParameters(rsaParam);
            PublicKey = publicKey;
        }

        /// <summary>
        /// Uses public Key to encrypt the Data
        /// </summary>
        /// <param name="data">The Data to encrypt</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] _data)
        {
            using (RSACryptoServiceProvider newRsa = new RSACryptoServiceProvider(keySize))
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
            using (RSACryptoServiceProvider newRsa = new RSACryptoServiceProvider(keySize))
            {
                newRsa.ImportParameters(rsa.ExportParameters(true));
                return newRsa.Decrypt(_data, true);
            }
        }
    }
}
