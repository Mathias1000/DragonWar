using System.Net.Sockets;
using DragonWar.Networking.Network.TCP.Client;

namespace DragonWar.LobbyClient.Network
{
    
    public class LobbySession : LobbyClientBase
    {
        public LobbySession(Socket mSocket) : base(mSocket)
        {
        }


    }
}
