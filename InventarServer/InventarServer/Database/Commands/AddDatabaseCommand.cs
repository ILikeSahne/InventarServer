using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class AddDatabaseCommand : Command
    {
        public string DatabaseName { get; set; }
        public string User { get; set; }
        public string PW { get; set; }
        public string Folder { get; set; }

        public AddDatabaseCommand() : this("", "", "", "") { }

        public AddDatabaseCommand(string _databaseName, string _user, string _pw, string _folder) : base(30, 30)
        {
            DatabaseName = _databaseName;
            User = _user;
            PW = _pw;
            Folder = _folder;
        }

        public override Error HandleCommandData(RSAHelper _helper)
        {
            try
            {
                ASCIIEncoding an = new ASCIIEncoding();
                DatabaseName = an.GetString(_helper.ReadByteArray());
                User = an.GetString(_helper.ReadByteArray());
                PW = an.GetString(_helper.ReadByteArray());
                Folder = an.GetString(_helper.ReadByteArray());
                InventarServer.GetDatabase().AddDatabase(new Database(new DatabaseLocation(DatabaseName, Folder, 0)));
            }
            catch (Exception e)
            {
                return new Error(ErrorType.COMMAND_ERROR, CommandErrorType.EQUIPMENT_DATA_CORRUPTED, e);
            }
            return Error.NO_ERROR;
        }

        public override void SendCommandResponse(RSAHelper _helper)
        {
            byte[] data = { ResponseToID(ValidateDatabase().ToString()) };
            _helper.WriteByteArray(data);
        }

        private AddDatabaseCommandError ValidateDatabase()
        {
            return AddDatabaseCommandError.NO_ERROR;
        }

        public override string[] GetResponses()
        {
            return Enum.GetNames(typeof(AddDatabaseCommandError));
        }
    }

    enum AddDatabaseCommandError
    {
        NO_ERROR,
        INVALID_NAME
    }
}
