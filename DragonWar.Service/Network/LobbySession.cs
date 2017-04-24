using DragonWar.Networking.Network.TCP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonWar.Networking.Network.TCP.Client;

namespace DragonWar.Service.Network
{
    public class LobbySession : LobbyClientBase
    {
        public LobbySession(Socket client) : base(client)
        {
        }
    }
}
