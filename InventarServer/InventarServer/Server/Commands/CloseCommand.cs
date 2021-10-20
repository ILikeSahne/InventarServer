using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    /// <summary>
    /// Is used to properly close an open connection
    /// </summary>
    class CloseCommand : Command
    {
        public CloseCommand() : base("Close")
        { }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public override void Execute(StreamHelper _helper, Client _c)
        {
            _c.Close();
        }
    }
}
