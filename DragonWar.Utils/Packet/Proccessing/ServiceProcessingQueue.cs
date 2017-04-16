using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Utils.Packet
{
    public class ServiceProcessingQueue<TServiceSession> : DataProcessingQueue<TServiceSession, ServicePacket> 
        where TServiceSession : ServiceSessionBase
    {
      
    }
}
