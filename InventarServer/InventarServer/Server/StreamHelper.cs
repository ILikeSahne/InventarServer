using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarServer
{
    /// <summary>
    /// Helps with communication
    /// </summary>
    class StreamHelper
    {
        /// <summary>
        /// Helps with RSA encryption/decryption
        /// </summary>
        public RSAHelper Helper { get; }

        private ASCIIEncoding ascii;

        /// <summary>
        /// Stores and initializes data
        /// </summary>
        /// <param name="_helper">Helps with RSA </param>
        public StreamHelper(RSAHelper _helper)
        {
            Helper = _helper;
            ascii = new ASCIIEncoding();
        }

        /// <summary>
        /// Sends a byte array
        /// </summary>
        /// <param name="_bytes">Byte array to send</param>
        public void SendByteArray(byte[] _bytes)
        {
            Helper.WriteByteArray(_bytes);
        }

        /// <summary>
        /// Reads a byte array
        /// </summary>
        /// <returns>The read byte array</returns>
        public byte[] ReadByteArray()
        {
            return Helper.ReadByteArray();
        }

        /// <summary>
        /// Sends a string
        /// </summary>
        /// <param name="_s">String to send</param>
        public void SendString(string _s)
        {
            SendByteArray(ascii.GetBytes(_s));
        }

        /// <summary>
        /// Reads a string
        /// </summary>
        /// <returns>The read string</returns>
        public string ReadString()
        {
            return ascii.GetString(ReadByteArray());
        }

        /// <summary>
        /// Sends an int
        /// </summary>
        /// <param name="_x">Int to send</param>
        public void SendInt(int _x)
        {
            SendByteArray(BitConverter.GetBytes(_x));
        }

        /// <summary>
        /// Reads an int
        /// </summary>
        /// <returns>The read int</returns>
        public int ReadInt()
        {
            byte[] array = ReadByteArray();
            return BitConverter.ToInt32(array, 0);
        }
    }
}
