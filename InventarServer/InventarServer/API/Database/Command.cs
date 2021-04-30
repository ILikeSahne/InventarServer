using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InventarAPI
{
    class Command
    {
        public CommandType Type { get; set; }
        public byte Number { get; set; }
        public byte Number2 { get; set; }

        public Command(CommandType _type, byte _number, byte _number2)
        {
            Type = _type;
            Number = _number;
            Number2 = _number2;
        }

        public bool IsCommand(byte[] _data)
        {
            return _data[0] == Number && _data[1] == Number2;
        }

        public CommandError SendCommand(RSAHelper _helper)
        {
            CommandError e = SendCommandType(_helper);
            if (!e)
            {
                return e;
            }
            e = SendCommandData(_helper);
            if (!e)
            {
                return e;
            }
            byte[] data = _helper.ReadByteArray();
            e = HandleResponse(data[0]);
            if (!e)
            {
                return e;
            }
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public CommandError HandleCommand(RSAHelper _helper)
        {
            CommandError e = HandleCommandData(_helper);
            if (!e)
            {
                return e;
            }
            e = SendCommandResponse(_helper);
            if (!e)
            {
                return e;
            }

            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public CommandError SendCommandType(RSAHelper _helper)
        {
            byte[] _data = { Number, Number2 };
            _helper.WriteByteArray(_data);
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public virtual CommandError SendCommandData(RSAHelper _helper)
        {
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public virtual CommandError HandleCommandData(RSAHelper _helper)
        {
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public virtual CommandError SendCommandResponse(RSAHelper _helper)
        {
            byte[] data = { 0 };
            _helper.WriteByteArray(data);
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public CommandError HandleResponse(int _id)
        {
            string[] responses = GetResponses();
            for (int i = 0; i < responses.Length; i++)
            {
                if (_id == i)
                    return new CommandError(CommandErrorType.COMMAND_FAILED, new Exception(responses[i]));
            }
            return new CommandError(CommandErrorType.UNKNOWN_RESPONSE, null);
        }

        public byte ResponseToID(string _response)
        {
            string[] responses = GetResponses();
            for (int i = 0; i < responses.Length; i++)
            {
                if (responses[i] == _response)
                    return (byte) i;
            }
            return 0;
        }

        public virtual string[] GetResponses()
        {
            string[] data = new string[1];
            data[0] = "NO_ERROR";
            return data;
        }
    }

    enum CommandType
    {
        ADD_EQUIPMENT
    }

    class CommandError
    {
        /// <summary>
        /// Type of Error
        /// </summary>
        public CommandErrorType Type { get; }
        /// <summary>
        /// Thrown Exception
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Saves values
        /// </summary>
        /// <param name="_type">Type of Error</param>
        /// <param name="_e">Thrown Exception</param>
        public CommandError(CommandErrorType _type, Exception _e)
        {
            Type = _type;
            Exception = _e;
        }

        /// <summary>
        /// Writes the Error to the Console (only when in DEBUG mode)
        /// </summary>
        public void PrintError()
        {
            InventarAPI.WriteLine("CommandError: " + ToString());
            StackFrame stackFrame = new StackFrame(1, true);
            string filename = stackFrame.GetFileName();
            int line = stackFrame.GetFileLineNumber();
            string method = stackFrame.GetMethod().ToString();
            InventarAPI.WriteLine("{0}:{1}, {2}", Path.GetFileName(filename), line, method);
        }

        /// <summary>
        /// Returns the Error as a String:
        ///     "TypeOfError: ExceptionMessage"
        /// </summary>
        /// <returns>"TypeOfError: ExceptionMessage"</returns>
        public override string ToString()
        {
            if (Exception != null)
                return Type + ": " + Exception.Message;
            else
                return Type.ToString();
        }

        /// <summary>
        /// Returns true if there was no Error, otherwise it returns false
        /// </summary>
        /// <param name="e">ServerError</param>
        public static implicit operator bool(CommandError e)
        {
            return e.Type == CommandErrorType.NO_ERROR;
        }
    }

    enum CommandErrorType
    {
        NO_ERROR,
        SYNTAX_ERROR,
        COMMAND_FAILED,
        UNKNOWN_RESPONSE,
        EQUIPMENT_DATA_CORRUPTED
    }
}
