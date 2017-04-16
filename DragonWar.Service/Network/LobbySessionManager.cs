using DragonWar.Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.Network
{
    [ServerModule(ServerType.Match,InitializationStage.Networking)]
    public class LobbySessionManager : SessionManagerBase<LobbySession>
    {
        public static LobbySessionManager Instance { get; private set; }

        public LobbySessionManager(ushort MaxSessions) : base(MaxSessions)
        {
        }

        [InitializerMethod]
        public static bool InitializedManager()
        {
            Instance = new LobbySessionManager(ServiceConfiguration.Instance.GameServer.MaxConnection);
            return true;
        }
    }
}
