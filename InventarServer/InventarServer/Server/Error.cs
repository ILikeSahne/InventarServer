using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Custom Error
    /// </summary>
    [Serializable]
    class Error : Exception
    {
        /// <summary>
        /// Error without Info
        /// </summary>
        public Error() : base() { }
        /// <summary>
        /// Error with custom message
        /// </summary>
        /// <param name="_message">Error message</param>
        public Error(string _message) : base(_message) { }
        /// <summary>
        /// Error with custom message and inner exception
        /// </summary>
        /// <param name="_message">Error message</param>
        /// <param name="_inner">Inner exception</param>
        public Error(string _message, Exception _inner) : base(_message, _inner) { }

        /// <summary>
        /// Used to serialize Error
        /// </summary>
        protected Error(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
