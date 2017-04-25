using DragonWar.Networking.Network.TCP.Client;
using DragonWar.Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.Network
{
    [ServerModule(ServerType.Service,InitializationStage.InternNetwork)]
    public class LobbySessionManager : LobbySessionManagerBase<LobbySession>
    {
        public static LobbySessionManager Instance { get; set; }

        public LobbySessionManager(ushort MaxSessions) : base(MaxSessions)
        {
        }



        [InitializerMethod]
        public static bool Initialized()
        {
            Instance = new LobbySessionManager(ServiceConfiguration.Instance.GameServer.MaxConnection);
            return true;
        }
    }

   
}
