using DragonWar.Game.Server;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.InternNetwork
{
    [ServerModule(ServerType.Service, InitializationStage.Logic)]
    public class MatchServerManager
    {

        public static MatchServerManager Instance { get; set; }

        private ConcurrentDictionary<int, MatchServerInfo> MatchServerInfoBySessionId { get; set; }


        public MatchServerManager()
        {
            MatchServerInfoBySessionId = new ConcurrentDictionary<int, MatchServerInfo>();
        }

        public bool GetServerBySessionId(int SessionId, out MatchServerInfo Info) => MatchServerInfoBySessionId.TryGetValue(SessionId, out Info);

        public bool AddServerInfo(MatchServerInfo Info) => MatchServerInfoBySessionId.TryAdd(Info.SessionId, Info);

        [InitializerMethod]
        public static bool Initalize()
        {
            Instance = new MatchServerManager();

            return true;
        }
    }
}
