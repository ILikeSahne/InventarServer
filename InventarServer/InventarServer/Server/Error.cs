using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    [Serializable]
    class Error : Exception
    {
        public Error() : base() { }
        public Error(string _message) : base(_message) { }
        public Error(string _message, Exception _inner) : base(_message, _inner) { }

        protected Error(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
