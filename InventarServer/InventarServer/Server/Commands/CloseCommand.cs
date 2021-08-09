using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class CloseCommand : Command
    {
        public CloseCommand() : base("Close")
        { }

        public override void Execute(StreamHelper _helper, Client _c)
        {
            _c.Close();
        }
    }
}
