using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonWar.Networking.Network.Processing;
using DragonWar.MatchServer.Config;
using DragonWar.Networking.Packet.Service.Authentication;

namespace DragonWar.MatchServer.InternNetwork
{
    [ServerModule(ServerType.Match,InitializationStage.Networking)]
    public class ServiceClient : ServiceClientBase
    {
        public  static ServiceClient Instance { get; set; }

        private DataProcessingQueue<ServiceClientBase, ServicePacket> ProcessingQueue { get; set; }

        public ServiceClient(Socket mSocket) : base(mSocket)
        {
            ProcessingQueue = new DataProcessingQueue<ServiceClientBase, ServicePacket>();
            ProcessingQueue.Start();
            NewProcessingInfo += info => ProcessingQueue.EnqueueProcessingInfo(info);
        }

        [InitializerMethod]
        public static bool Initialize()
        {

            if (Instance != null)
                throw new InvalidOperationException();

            Instance = new ServiceClient(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            Instance.TryConnectToLogin(MatchServerConfiguration.Instance.ConnectInfo.ConnectIP, MatchServerConfiguration.Instance.ConnectInfo.ConnectPort);
            Instance.StartRecv();
            if (!Instance.IsConnected)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Failed to Connect ServiceServer");
                return false;
            }

            Instance.SendPacket(new Auth_ACK
            {
                Password = MatchServerConfiguration.Instance.ConnectInfo.ConnectPassword
            });

            return true;
        }
    }
}
