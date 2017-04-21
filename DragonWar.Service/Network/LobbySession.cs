using System.Net.Sockets;

using DragonWar.Networking.Network;

namespace DragonWar.Service.Network
{
    public class LobbySession : GameSessionBase
    {
        public LobbySession(Socket sock) : base(sock)
        {
        }

      
    }
}
