using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InventarServer
{
    class CommandManager {

        private List<Command> commands;

        public CommandManager()
        {
            commands = new List<Command>();
            commands.Add(new AddEquipmentCommand());
        }

        public Error ExecuteCommand(byte[] _data, RSAHelper _helper)
        {
            foreach(Command c in commands)
            {
                if(c.IsCommand(_data))
                {
                    return c.HandleCommand(_helper);
                }
            }
            return Error.NO_ERROR;
        }
    }

    class Command
    {
        public byte Number { get; set; }
        public byte Number2 { get; set; }

        public Command(byte _number, byte _number2)
        {
            Number = _number;
            Number2 = _number2;
        }

        public bool IsCommand(byte[] _data)
        {
            return _data[0] == Number && _data[1] == Number2;
        }

        public Error HandleCommand(RSAHelper _helper)
        {
            Error e = HandleCommandData(_helper);
            if (!e)
            {
                return new Error(ErrorType.COMMAND_ERROR, CommandErrorType.COMMAND_DATA_ERROR, e);
            }
            SendCommandResponse(_helper);
            return Error.NO_ERROR;
        }

        public virtual Error SendCommandData(RSAHelper _helper)
        {
            return Error.NO_ERROR;
        }

        public virtual Error HandleCommandData(RSAHelper _helper)
        {
            return Error.NO_ERROR;
        }

        public virtual void SendCommandResponse(RSAHelper _helper)
        {
            byte[] data = { 0, 0 };
            Console.WriteLine(data.Length);
            _helper.WriteByteArray(data);
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

    enum CommandErrorType
    {
        NO_ERROR,
        COMMAND_NOT_FOUND,
        COMMAND_DATA_ERROR,
        UNKNOWN_RESPONSE,
        EQUIPMENT_DATA_CORRUPTED
    }
}
