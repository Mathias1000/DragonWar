using DragonWar.Networking.Handling.Store;
using DragonWar.Networking.Network;
using DragonWar.Networking.Network.TCP.Client;

namespace DragonWar.Networking.Packet
{
    public class ServiceProcessingQueue<TServiceSession> : DataProcessingQueue<TServiceSession, ServicePacket> 
        where TServiceSession : ServiceClientBase
    {

        protected override void ProcessInfo(DataProcessingInfo<TServiceSession, ServicePacket> info)
        {
            ServiceHandlerStore.Instance.HandlePacket(info.Packet, info.Session);
        }
    }
}
