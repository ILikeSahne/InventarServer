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

        public AddEquipmentCommand(string _databaseName, string _user, string _pw, Equipment _e) : base(10, 10)
        {
            DatabaseName = _databaseName;
            User = _user;
            PW = _pw;
            Equipment = _e;
        }

        public override Error HandleCommandData(RSAHelper _helper)
        {
            try
            {
                ASCIIEncoding an = new ASCIIEncoding();
                DatabaseName = an.GetString(_helper.ReadByteArray());
                User = an.GetString(_helper.ReadByteArray());
                PW = an.GetString(_helper.ReadByteArray());
                Equipment = new Equipment().FromByteArray(_helper.ReadByteArray());
                Database d = InventarServer.GetDatabase().GetDatabase(DatabaseName);
                if(d == null)
                {
                    return new Error(ErrorType.COMMAND_ERROR, EquipmentCommandError.DATABASE_DOESNT_EXIST);
                }
                d.AddEquipment(Equipment);
            }
            catch (Exception e)
            {
                return new Error(ErrorType.COMMAND_ERROR, EquipmentCommandError.UNKNOWN_ERROR, e);
            }
            return Error.NO_ERROR;
        }

        public override void SendCommandResponse(RSAHelper _helper)
        {
            int id = ResponseToID(ValidateEquipment().ToString());
            byte[] data = { (byte) (id & 0xFF), (byte) ((id >> 8) & 0xFF) };
            _helper.WriteByteArray(data);
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
        DATABASE_DOESNT_EXIST,
        UNKNOWN_ERROR,
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
