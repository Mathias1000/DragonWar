using DragonWar.Networking.Network.TCP.Server;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonWar.Service.Config;

namespace DragonWar.Service.InternNetwork
{
    [ServerModule(ServerType.Service, InitializationStage.Networking)]
    public class ServiceServer : ServiceServerBase
    {

        private static ServiceServer Instance { get; set; }

        public ServiceServer(string ip, int port, int WorkCount) : base(ip, port, WorkCount)
        {
        }

        public override async Task DoWork(Socket client)
        {
            var mSession = new ServiceSession(client);

            if (!ServiceSessionManager.Instance.AddSession(mSession))
            {
                SocketLog.Write(SocketLogLevel.Warning, $"Refuse {client.RemoteEndPoint} Connection MaxService Connection Reached!");
                mSession.Close();
                return;
            }

            mSession.NewProcessingInfo +=
               info => ProcessingQueue.EnqueueProcessingInfo(info);

            await Task.Factory.StartNew(mSession.StartRecv);

        }

        [InitializerMethod]
        public static bool Initialize()
        {
            Instance = new ServiceServer(ServiceConfiguration.Instance.Service.ListenerIP,
                ServiceConfiguration.Instance.Service.Port,
                ServiceConfiguration.Instance.Service.NetworThreads);
            
            Instance.Start();

            return true;
        }
    }
}
