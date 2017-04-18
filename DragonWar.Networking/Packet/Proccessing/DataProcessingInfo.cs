using DragonWar.Networking.Network;

namespace DragonWar.Networking.Packet
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
