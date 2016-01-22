using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Lib.Network
{
    public class NewConnectionEventArgs : EventArgs
    {
        public Connection Connection { get; private set; }

        public NewConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }
    }
}
