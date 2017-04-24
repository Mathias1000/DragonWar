using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP
{
    public class SocketDisconnectArgs : EventArgs
    {
       public SocketError Error { get; private set; }

        public string Message { get; private set; }
        public SocketDisconnectArgs(SocketError Error,string msg = "")
        {
            this.Message = msg;
            this.Error = Error;
        }
    }
}
