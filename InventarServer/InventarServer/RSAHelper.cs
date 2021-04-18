using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace InventarServer
{
    class RSAHelper
    {
        private ASCIIEncoding ascii;
        private NetworkStream stream;

        private RSA encryptRSA, decryptRSA;

        public RSAHelper(NetworkStream _stream)
        {
            stream = _stream;
            ascii = new ASCIIEncoding();
        }

        public RSAError SetupServer()
        {
            // Sending Server public key
            decryptRSA = new RSA();
            byte[] serverPublicKey = ascii.GetBytes(decryptRSA.PublicKey);
            write2Bytes(serverPublicKey.Length);
            writeBytes(serverPublicKey);

            // Reading Client public key
            int len = read2Bytes();
            Console.WriteLine(len);
            string clientPublicKey = ascii.GetString(readBytes(len));
            Console.WriteLine(clientPublicKey);
            encryptRSA = new RSA(clientPublicKey);

            // Write Response
            byte[] data = new byte[64];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i * 2);
            }
            writeEncryptedBytes(data);
            // Wait for Response
            byte[] response = readDecryptedBytes();
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i] != data[i] * 2)
                    return RSAError.RESPONSE_ERROR;
            }
            return RSAError.NO_ERROR;
        }

        public RSAError SetupClient()
        {
            try
            {
                decryptRSA = new RSA();

                // Reading Server public Key
                int len = read2Bytes();
                string serverPublicKey = ascii.GetString(readBytes(len));
                encryptRSA = new RSA(serverPublicKey);

                // Sending Client public Key
                byte[] clientPublicKey = ascii.GetBytes(decryptRSA.PublicKey);
                write2Bytes(clientPublicKey.Length);
                writeBytes(clientPublicKey);
            }
            catch (Exception e)
            {
                return RSAError.CONNECTION_ERROR;
            }
            // Wait for response
            byte[] response = readDecryptedBytes();
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i] != i * 2)
                    return RSAError.RESPONSE_ERROR;
                response[i] *= 2;
            }
            //Write response
            writeEncryptedBytes(response);

            return RSAError.NO_ERROR;
        }

        private byte[] readBytes(int amount)
        {
            byte[] data = new byte[amount];
            stream.Read(data, 0, amount);
            return data;
        }

        private int read2Bytes()
        {
            byte[] data = readBytes(2);
            return (data[0] << 8) + data[1];
        }

        private void writeBytes(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        private void write2Bytes(int x)
        {
            byte[] data = new byte[2];
            data[0] = (byte)((x >> 8) & 0xFF);
            data[1] = (byte)(x & 0xFF);
            writeBytes(data);
        }

        private void writeEncryptedBytes(byte[] data)
        {
            writeBytes(encryptRSA.Encrypt(data));
        }

        private byte[] readDecryptedBytes()
        {
            return decryptRSA.Decrypt(readBytes(128));
        }
    }

    enum RSAError
    {
        NO_ERROR,
        CONNECTION_ERROR,
        RESPONSE_ERROR
    }
}
