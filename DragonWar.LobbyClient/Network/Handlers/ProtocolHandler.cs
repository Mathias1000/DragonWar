using DragonWar.Networking.Packet.Lobby.Protocol;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.LobbyClient.Network.Handlers
{
    [LobbyHandlerClass(LobbyHeaderType.Protocol)]
    public class ProtocolHandler
    {

        [LobbyHandler((byte)ProtocolTypes.Handshake)]
        public static void HandleServerHandshake(LobbySession mSession, LobbyHandShake mPacket)
        {
          //TODO Crypting
        }
    }
}
