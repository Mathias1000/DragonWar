using DragonWar.Networking.Network;

namespace DragonWar.Networking.Packet
{
    public class ServiceDataProccessingInfo<Session> : DataProcessingInfo<Session, IServicePacket> where Session : ServiceSessionBase
    {
        public ServiceDataProccessingInfo(Session session, IServicePacket pPacket) : base(session, pPacket)
        {
        }
    }
}
