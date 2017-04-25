using DragonWar.Networking.Network.TCP.Client;
using DragonWar.Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.InternNetwork
{
    [ServerModule(ServerType.Service,InitializationStage.InternNetwork)]
    public class ServiceSessionManager : ServiceSessionManagerBase<ServiceSession>
    {

        public static ServiceSessionManager Instance { get; set; }
        public ServiceSessionManager(ushort MaxSessions) : base(MaxSessions)
        {
        }

        [InitializerMethod]
        public static bool Initialize()
        {
            Instance = new ServiceSessionManager(ServiceConfiguration.Instance.Service.MaxConnection);
            return true;
        }
    }
}
