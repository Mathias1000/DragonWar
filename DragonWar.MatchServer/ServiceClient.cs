using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace DragonWar.MatchServer
{
    public class ServiceClient : ServiceClientBase
    {
        public ServiceClient(Socket mSocket) : base(mSocket)
        {
        }
    }
}
