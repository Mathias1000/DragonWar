using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public class ServiceSessionBase : SessionBase
    {

     
        public ServiceSessionBase(Socket sock)
             : base(sock)
        {
            if (sock == null)
                throw new ArgumentNullException();
        
        }

        public virtual void SendPacket(ServicePacket pPacket)
        {
            if (!pPacket.Read().GetType().IsSerializable)
            {
                SocketLog.Write(SocketLogLevel.Warning, "Packet {0} is not IsSerializable can not send", pPacket.GetType());
                return;
            }


            Send(pPacket.Write());
        }

    }
}
