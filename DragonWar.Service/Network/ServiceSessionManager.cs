using DragonWar.Service.Config;

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
