using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class CreateNewDatabaseCommand : Command
    {
        public CreateNewDatabaseCommand() : base("CreateNewDatabase")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            if (!_u.IsSuperAdminUser())
            {
                SendNoPermissionMessage(_helper);
                return;
            }
            SendOKMessage(_helper);

            string database = _helper.ReadString();
            string email = _helper.ReadString();
            string username = _helper.ReadString();
            string password = _helper.ReadString();

            Database d = new Database(database);
            if (d.Exists()) {
                _helper.SendString("Databases already Exists");
                return;
            }

            d.CreateNewDatabase();

            ValidateError e = d.AddUser(email, username, password);
            if (e != ValidateError.NONE) {
                _helper.SendString(e.ToString());
                return;
            }

            SendOKMessage(_helper);
        }
    }
}
