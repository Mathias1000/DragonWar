using DragonWar.Networking.Network.TCP.Server;
using DragonWar.Service.Config;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DragonWar.Service.Network
{
    [ServerModule(ServerType.Service, InitializationStage.Networking)]
    public class LobbyServer : LobbyServerBase<LobbySession>
    {
        public static LobbyServer Instance { get; private set; }

        public LobbyServer(string ip, int port,int WorkCount) : base(ip, port,WorkCount)
        {
        }

        public override async Task DoWork(Socket client)
        {
            var mSession = new LobbySession(client);
            bool Res = await Task.FromResult(LobbySessionManager.Instance.AddSession(mSession));

            SendVerfiryConnectPacket(mSession, Res);

            if (!Res)  mSession.Close();
 

                mSession.NewProcessingInfo +=
                    info => ProcessingQueue.EnqueueProcessingInfo(info);

                await Task.Factory.StartNew(mSession.StartRecv);
        }

        [InitializerMethod]
        public static bool Initialize()
        {
            Instance = new LobbyServer(ServiceConfiguration.Instance.GameServer.ListenerIP,
                ServiceConfiguration.Instance.GameServer.Port,
                ServiceConfiguration.Instance.GameServer.NetworThreads);

            Instance.Start();

            return true;
        }


    }
}
