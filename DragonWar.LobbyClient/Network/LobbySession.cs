using DragonWar.Networking.Network;
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
