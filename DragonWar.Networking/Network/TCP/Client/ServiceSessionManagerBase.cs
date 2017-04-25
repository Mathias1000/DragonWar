using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP.Client
{
    public class ServiceSessionManagerBase<TClient> : SessionManagerBase<TClient> where TClient : ServiceClientBase
    {
        public ServiceSessionManagerBase(ushort MaxSessions) : base(MaxSessions)
        {
        }
    }
}
