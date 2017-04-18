using System.Net.Sockets;
using DragonWar.Networking.Network;

namespace DragonWar.MatchServer.Network
{
    public class GameSession : GameSessionBase
    {
        public GameSession(Socket sock) : base(sock)
        {
        }
    }
}
