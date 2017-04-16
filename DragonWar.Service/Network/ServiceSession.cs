using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace DragonWar.Service.Network
{
    public class ServiceSession : ServiceSessionBase
    {
        public ServiceSession(Socket sock) : base(sock)
        {
        }
    }
}
