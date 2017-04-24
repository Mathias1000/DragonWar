using DragonWar.Networking.Network;
using DragonWar.Networking.Network.TCP.Client;

namespace DragonWar.Networking.Network.Processing
{
    public class DataProcessingInfo<SessionType, PacketType> where SessionType : ClientBase
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
