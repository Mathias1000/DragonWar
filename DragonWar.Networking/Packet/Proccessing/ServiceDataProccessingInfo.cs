using DragonWar.Networking.Network;

namespace DragonWar.Networking.Packet
{
    public class ServiceDataProccessingInfo<Session> : DataProcessingInfo<Session, ServicePacket> where Session : ServiceSessionBase
    {
        public ServiceDataProccessingInfo(Session session, ServicePacket pPacket) : base(session, pPacket)
        {
        }
    }
}
