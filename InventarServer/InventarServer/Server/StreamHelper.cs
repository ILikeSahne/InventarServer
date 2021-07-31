using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarServer
{
    class StreamHelper
    {
        private RSAHelper helper;
        private ASCIIEncoding ascii;

        public StreamHelper(RSAHelper _helper)
        {
            helper = _helper;
            ascii = new ASCIIEncoding();
        }

        public void SendString(string _s)
        {
            helper.WriteByteArray(ascii.GetBytes(_s));
        }

        public string ReadString()
        {
            return ascii.GetString(helper.ReadByteArray());
        }
    }
}
