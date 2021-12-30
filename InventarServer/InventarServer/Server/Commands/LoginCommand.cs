using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class LoginCommand : Command
    {
        public LoginCommand() : base("Login")
        { }

        public override void Execute(User _u, StreamHelper _helper, Client _c)
        {
            
        }
    }
}
