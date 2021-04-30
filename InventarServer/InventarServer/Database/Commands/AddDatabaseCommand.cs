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

        public AddDatabaseCommand(string _databaseName, string _user, string _pw, string _folder) : base(CommandType.ADD_DATABASE, 30, 30)
        {
            DatabaseName = _databaseName;
            User = _user;
            PW = _pw;
            Folder = _folder;
        }

        public override CommandError SendCommandData(RSAHelper _helper)
        {
            try
            {
                ASCIIEncoding an = new ASCIIEncoding();
                _helper.WriteByteArray(an.GetBytes(DatabaseName));
                _helper.WriteByteArray(an.GetBytes(User));
                _helper.WriteByteArray(an.GetBytes(PW));
                _helper.WriteByteArray(an.GetBytes(Folder));
            }
            catch (Exception e)
            {
                return new CommandError(CommandErrorType.EQUIPMENT_DATA_CORRUPTED, e);
            }
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public override CommandError HandleCommandData(RSAHelper _helper)
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
                return new CommandError(CommandErrorType.EQUIPMENT_DATA_CORRUPTED, e);
            }
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        public override CommandError SendCommandResponse(RSAHelper _helper)
        {
            byte[] data = { ResponseToID(ValidateEquipment().ToString()) };
            _helper.WriteByteArray(data);
            return new CommandError(CommandErrorType.NO_ERROR, null);
        }

        private EquipmentCommandError ValidateEquipment()
        {
            return EquipmentCommandError.NO_ERROR;
        }

        public override string[] GetResponses()
        {
            return Enum.GetNames(typeof(EquipmentCommandError));
        }
    }

    enum AddDatabaseCommandError
    { 
        NO_ERROR,
        INVALID_NUMBER_ERROR,
        INVALID_SECOND_NUMBER_ERROR,
        INVALID_CURRENT_NUMBER_ERROR,
        INVALID_ACTIVATION_DATE_ERROR,
        INVALID_NAME_ERROR,
        INVALID_SERIAL_NUMBER_ERROR,
        INVALID_COST_ERROR,
        INVALID_BOOK_VALUE_ERROR,
        INVALID_CURRENCY_ERROR,
        INVALID_KFZ_LICENSE_PLATE_ERROR,
        INVALID_ROOM_ERROR,
        INVALID_ROOM_NAME_ERROR
    }
}
