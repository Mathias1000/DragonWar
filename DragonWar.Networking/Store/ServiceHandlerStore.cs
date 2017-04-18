using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Handling.Store
{
    [ServerModule(ServerType.Service, InitializationStage.Networking)]
    public class ServiceHandlerStore
    {
        public static ServiceHandlerStore Instance { get; private set; }

    }
}
