using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarServer
{
    class StreamHelper
    {
        public RSAHelper Helper { get; }
        private ASCIIEncoding ascii;

        public StreamHelper(RSAHelper _helper)
        {
            Helper = _helper;
            ascii = new ASCIIEncoding();
        }

        public void SendString(string _s)
        {
            Helper.WriteByteArray(ascii.GetBytes(_s));
        }

        public string ReadString()
        {
            return ascii.GetString(Helper.ReadByteArray());
        }

        public void SendInt(int _x)
        {
            Helper.WriteByteArray(BitConverter.GetBytes(_x));
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(Helper.ReadByteArray(), 0);
        }
    }
}
