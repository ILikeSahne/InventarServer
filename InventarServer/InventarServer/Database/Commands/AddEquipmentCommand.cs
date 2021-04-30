using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class AddEquipmentCommand : Command
    {
        public string DatabaseName { get; set; }
        public string User { get; set; }
        public string PW { get; set; }
        public Equipment Equipment { get; set; }

        public AddEquipmentCommand() : this("", "", "", null) { }

        public AddEquipmentCommand(string _databaseName, string _user, string _pw, Equipment _e) : base(CommandType.ADD_EQUIPMENT, 10, 10)
        {
            DatabaseName = _databaseName;
            User = _user;
            PW = _pw;
            Equipment = _e;
        }

        public override CommandError SendCommandData(RSAHelper _helper)
        {
            try
            {
                ASCIIEncoding an = new ASCIIEncoding();
                _helper.WriteByteArray(an.GetBytes(DatabaseName));
                _helper.WriteByteArray(an.GetBytes(User));
                _helper.WriteByteArray(an.GetBytes(PW));
                _helper.WriteByteArray(Equipment.ToByteArray());
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
                Equipment = new Equipment().FromByteArray(_helper.ReadByteArray());
                InventarServer.GetDatabase().GetDatabase(DatabaseName).AddEquipment(Equipment);
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

    enum EquipmentCommandError
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
