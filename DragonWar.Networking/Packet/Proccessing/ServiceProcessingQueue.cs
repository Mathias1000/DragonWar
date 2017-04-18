using DragonWar.Networking.Network;

namespace DragonWar.Networking.Packet
{
    public class ServiceProcessingQueue<TServiceSession> : DataProcessingQueue<TServiceSession, IServicePacket> 
        where TServiceSession : ServiceSessionBase
    {
      
    }
}
