using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Utils.Packet
{
    public class ServiceDataProccessingInfo<Session> : DataProcessingInfo<Session, ServicePacket> where Session : ServiceSessionBase
    {
        public ServiceDataProccessingInfo(Session session, ServicePacket pPacket) : base(session, pPacket)
        {
        }
    }
}
