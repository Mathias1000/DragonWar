using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet.Lobby.Server
{
    [Serializable]
    public class ServerStatusPacket : LobbyPacket
    {
        public override LobbyHeaderType Header => LobbyHeaderType.Protocol;

        public override ushort HandlingType => (byte)ServerStatusType.ServerStatus;

        public ServerState State { get; set; }
    }
}
