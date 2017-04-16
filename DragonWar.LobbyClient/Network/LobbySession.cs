using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace DragonWar.LobbyClient.Network
{
    public class LobbySession : ServiceClient<LobbySession>
    {
        public LobbySession(Socket sock) : base(sock)
        {
        }

        protected override void ReceiveMessage(ServiceClient<LobbySession> client, MemoryStream packet)
        {
            
        }
    }
}
