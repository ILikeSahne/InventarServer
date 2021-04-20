using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace InventarServer
{
    class RSAHelper
    {
        private const int MAX_LENGTH = 86;

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
            Write2Bytes(serverPublicKey.Length);
            WriteBytes(serverPublicKey);

            // Reading Client public key
            int len = Read2Bytes();
            string clientPublicKey = ascii.GetString(ReadBytes(len));
            encryptRSA = new RSA(clientPublicKey);

            // Write Response
            byte[] data = new byte[10];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i * 4);
            }
            WriteEncryptedBytes(data);
            // Wait for Response
            byte[] response = ReadDecryptedBytes();
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i] != data[i] * 4)
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
                int len = Read2Bytes();
                string serverPublicKey = ascii.GetString(ReadBytes(len));
                encryptRSA = new RSA(serverPublicKey);

                // Sending Client public Key
                byte[] clientPublicKey = ascii.GetBytes(decryptRSA.PublicKey);
                Write2Bytes(clientPublicKey.Length);
                WriteBytes(clientPublicKey);
            }
            catch (Exception e)
            {
                return RSAError.CONNECTION_ERROR;
            }
            // Wait for response
            byte[] response = ReadDecryptedBytes();
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i] != i * 4)
                    return RSAError.RESPONSE_ERROR;
                response[i] *= 4;
            }
            //Write response
            WriteEncryptedBytes(response);

            return RSAError.NO_ERROR;
        }

        private byte[] ReadBytes(int _amount)
        {
            byte[] data = new byte[_amount];
            stream.Read(data, 0, _amount);
            return data;
        }

        private int Read2Bytes()
        {
            byte[] data = ReadBytes(2);
            return (data[0] << 8) + data[1];
        }

        private void WriteBytes(byte[] _data)
        {
            stream.Write(_data, 0, _data.Length);
        }

        private void Write2Bytes(int _x)
        {
            byte[] data = new byte[2];
            data[0] = (byte)((_x >> 8) & 0xFF);
            data[1] = (byte)(_x & 0xFF);
            WriteBytes(data);
        }

        private void WriteEncryptedBytes(byte[] _data)
        {
            if (_data.Length > MAX_LENGTH)
            {
                InventarServer.WriteLine("Error: too many bytes send at once, amount: {0} bytes", _data.Length);
                return;
            }
            WriteBytes(encryptRSA.Encrypt(_data));
        }

        private byte[] ReadDecryptedBytes()
        {
            return decryptRSA.Decrypt(ReadBytes(128));
        }

        public void WriteByteArray(byte[] _data)
        {
            int len = _data.Length;
            Console.WriteLine(len);
            byte[] lenBytes = new byte[]
            {
                (byte) ((len >> 24) & 0xFF),
                (byte) ((len >> 16) & 0xFF),
                (byte) ((len >> 8) & 0xFF),
                (byte) (len & 0xFF)
            };
            WriteEncryptedBytes(lenBytes);
            int amount = len / MAX_LENGTH + 1;
            for (int i = 0; i < amount; i++)
            {
                int pos = i * MAX_LENGTH;
                int newDataLenght = Math.Min(MAX_LENGTH, len - pos);
                Console.WriteLine(newDataLenght);
                byte[] newData = new byte[newDataLenght];
                for (int j = 0; j < newDataLenght; j++)
                {
                    newData[j] = _data[pos + j];
                }
                WriteEncryptedBytes(newData);
            }
        }

        public byte[] ReadByteArray()
        {
            byte[] lenBytes = ReadDecryptedBytes();
            int len = (lenBytes[0] << 24) + (lenBytes[1] << 16) + (lenBytes[2] << 8) + lenBytes[3];
            for(int i = 0; i < lenBytes.Length; i++)
            {
                Console.WriteLine(lenBytes[i]);
            }
            Console.WriteLine(len);
            byte[] data = new byte[len];
            int amount = len / MAX_LENGTH + 1;
            for (int i = 0; i < amount; i++)
            {
                int pos = i * MAX_LENGTH;
                byte[] newData = ReadDecryptedBytes();
                Console.WriteLine(newData.Length);
                for (int j = 0; j < newData.Length; j++)
                {
                    data[pos + j] = newData[j];
                }
            }
            return data;
        }

    }

    enum RSAError
    {
        NO_ERROR,
        CONNECTION_ERROR,
        RESPONSE_ERROR
    }
}
