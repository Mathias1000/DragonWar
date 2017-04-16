using DragonWar.Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.Network
{
    [GameServerModule(ServerType.Service, GameInitalStage.Logic)]
    public class ServiceSessionManager : SessionManagerBase<ServiceSession>
    {
        public static ServiceSessionManager Instance { get; private set; }

        public ServiceSessionManager(ushort MaxSessions) : base(MaxSessions)
        {
        }

        [InitializerMethod]
        public static bool InitialServiceManager()
        {
            Instance = new ServiceSessionManager(ServiceConfiguration.Instance.Service.MaxConnection);
            return true;
        }
    }
}
