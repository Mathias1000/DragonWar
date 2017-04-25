using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet.Service.Authentication
{
    [Serializable]
    public class Auth_RES : ServicePacket
    {
        public override ServiceHeaderType Header => ServiceHeaderType.Protocol;

        public override byte Handling => (byte)ConnectionTypes.Auth_RES;

        public bool IsOK { get; set; }

        public int SessionId { get; set; }
    }
}
