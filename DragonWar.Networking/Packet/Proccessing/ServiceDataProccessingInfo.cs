using DragonWar.Networking.Network;
using DragonWar.Networking.Network.TCP.Client;

namespace DragonWar.Networking.Packet
{
    public class ServiceDataProccessingInfo<Session> : DataProcessingInfo<Session, ServicePacket> where Session : ServiceClientBase
    {
        public ServiceDataProccessingInfo(Session session, ServicePacket pPacket) : base(session, pPacket)
        {
        }
    }
}
