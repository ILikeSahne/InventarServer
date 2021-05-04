using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InventarServer
{
    class Error
    {
        /// <summary>
        /// Is used if there is no Error to return
        /// </summary>
        public static Error NO_ERROR = new Error();

        /// <summary>
        /// The Type of the Error
        /// </summary>
        public ErrorType ErrorType;
        /// <summary>
        /// The Message of the Error
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The Exception of the Error (can be null)
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// The Child Error (can be null)
        /// </summary>
        public Error Next { get; set; }

        /// <summary>
        /// Creates a new Error with the ErrorType of NO_ERROR
        /// </summary>
        public Error() : this(ErrorType.NO_ERROR, "NO_ERROR", null, null)
        { }

        /// <summary>
        /// Creates a new Error with a specific Error Type and a custom message
        /// </summary>
        /// <param name="_errorType">The Type of the Error</param>
        /// <param name="_message">The custom message</param>
        public Error(ErrorType _errorType, object _message) : this(_errorType, _message, null, null)
        { }

        /// <summary>
        /// Creates a new Error with a specific Error Type, a custom message and an Exception
        /// </summary>
        /// <param name="_errorType">The Type of the Error</param>
        /// <param name="_message">The custom message</param>
        /// <param name="_ex">The Exception</param>
        public Error(ErrorType _errorType, object _message, Exception _ex) : this(_errorType, _message, _ex, null)
        { }

        /// <summary>
        /// Creates a new Error with a specific Error Type, a custom message and an Exception
        /// </summary>
        /// <param name="_errorType">The Type of the Error</param>
        /// <param name="_message">The custom message</param>
        /// <param name="_next">The Child Error</param>
        public Error(ErrorType _errorType, object _message, Error _next) : this(_errorType, _message, null, _next)
        { }

        /// <summary>
        /// Creates a new Error with a specific Error Type, a custom message, an Exception and a child Error
        /// </summary>
        /// <param name="_errorType">The Type of the Error</param>
        /// <param name="_message">The custom message</param>
        /// <param name="_ex">The Exception</param>
        /// <param name="_next">The Child Error</param>
        public Error(ErrorType _errorType, object _message, Exception _ex, Error _next)
        {
            ErrorType = _errorType;
            Message = _message.ToString();
            Exception = _ex;
            Next = _next;
        }

        /// <summary>
        /// Prints the Parent Error plus all the Child Errors
        /// </summary>
        public void PrintAllErrors()
        {
            Error e = this;
            while (e != null)
            {
                if (e.ErrorType == ErrorType.NO_ERROR)
                    return;
                e.PrintError(2);
                e = e.Next;
            }
        }

        /// <summary>
        /// Prints an Error plus the StackTrace
        /// </summary>
        public void PrintError(int amount)
        {
            InventarServer.WriteLine(ToString());
            StackFrame stackFrame = new StackFrame(amount, true);
            string filename = stackFrame.GetFileName();
            int line = stackFrame.GetFileLineNumber();
            string method = stackFrame.GetMethod().ToString();
            InventarServer.WriteLine("{0}:{1}, {2}", Path.GetFileName(filename), line, method);
        }

        /// <summary>
        /// Converts the Error into a string
        /// </summary>
        /// <returns>The Error as a string</returns>
        public override string ToString()
        {
            if (Exception != null)
                return ErrorType.ToString() + ": " + Message + ": " + Exception.Message;
            else
                return ErrorType.ToString() + ": " + Message;
        }

        /// <summary>
        /// Returns true if there was no Error
        /// </summary>
        /// <param name="_e">The Error to check</param>
        public static implicit operator bool(Error _e)
        {
            return _e.ErrorType == ErrorType.NO_ERROR;
        }
    }

    enum ErrorType
    {
        NO_ERROR,
        RSA_ERROR,
        DATABASE_ERROR,
        EQUIPMENT_ERROR,
        COMMAND_ERROR,
        SERVER_ERROR,
        CLIENT_ERROR
    }
}
