using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet.Service.Authentication
{
    public class Auth_ACK : ServicePacket
    {
        public override ServiceHeaderType Header => ServiceHeaderType.Connection;

        public override byte Handling => (byte)ConnectionTypes.Auth_ACK;

        public string Password { get; set; }
    }
}
