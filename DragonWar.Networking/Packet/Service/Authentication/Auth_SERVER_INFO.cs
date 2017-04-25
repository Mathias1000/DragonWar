using DragonWar.Game.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet.Service.Authentication
{
    [Serializable]
    public class Auth_SERVER_INFO : ServicePacket
    {
        public override ServiceHeaderType Header => ServiceHeaderType.Protocol;

        public override byte Handling => (byte)ConnectionTypes.Auth_SERVER_INFO;

        public MatchServerInfo ServerInfo { get; set; }
    }
}
