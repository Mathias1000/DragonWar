using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public class GameSessionBase : SessionBase
    {
        public ushort EncryptKey { get; set; }

        public GameSessionBase(Socket sock) : base(sock)
        {
        }

      

        public void SendPacket(LobbyPacket pPacket)
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
