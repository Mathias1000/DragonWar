using DragonWar.Networking.Network;
using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet.Proccessing
{
    public class LobbyProcessingQueue<TServiceSession> : DataProcessingQueue<TServiceSession, LobbyPacket>
          where TServiceSession : SessionBase
    {

        protected override void ProcessInfo(DataProcessingInfo<TServiceSession, LobbyPacket> info)
        {
            Store.LobbyHandlerStore.Instance.HandlePacket(info.Packet, info.Session);
        }
    }
}

