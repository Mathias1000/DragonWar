using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Utils.Packet
{
    public class DataProcessingInfo<SessionType, PacketType> where SessionType : SessionBase
    {
        public SessionType Session { get; private set; }
        public PacketType Packet { get; private set; }

        public DataProcessingInfo(SessionType session, PacketType pPacket)
        {
            Session = session;
            Packet = pPacket;
        }
    }
}
