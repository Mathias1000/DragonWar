using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonWar.Networking.Network.TCP;
using DragonWar.Networking.Packet.Service.Authentication;
using DragonWar.Service.InternNetwork.Handlers;

namespace DragonWar.Service.InternNetwork
{
    public class ServiceSession : ServiceClientBase
    {
        public bool Authenticated { get; set; }

        public ServiceSession(Socket mSocket) : base(mSocket)
        {
        }

        public override void HandlePacket<T>(T Packet)
        {
            if(Packet is Auth_ACK)
            {
                ProtocolHandler.HandleServerHandshake(this, Packet as Auth_ACK);
            }
            else if (Authenticated)
            {
                base.HandlePacket(Packet);
            }
            else
            {
                this.Close();
            }
        }

    }
}
