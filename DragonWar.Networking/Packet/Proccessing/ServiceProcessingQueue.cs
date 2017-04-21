using DragonWar.Networking.Handling.Store;
using DragonWar.Networking.Network;

namespace DragonWar.Networking.Packet
{
    public class ServiceProcessingQueue<TServiceSession> : DataProcessingQueue<TServiceSession, ServicePacket> 
        where TServiceSession : ServiceSessionBase
    {

        protected override void ProcessInfo(DataProcessingInfo<TServiceSession, ServicePacket> info)
        {
            ServiceHandlerStore.Instance.HandlePacket(info.Packet, info.Session);
        }
    }
}
