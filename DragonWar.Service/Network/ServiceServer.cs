using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using DragonWar.Service.Config;
using DragonWar.Service.ServerConsole.Title;
using DragonWar.Utils.Packet;

namespace DragonWar.Service.Network
{
    //Recvive From MatchServers
    [GameServerModule(ServerType.Service,GameInitalStage.Network)]
    public class ServiceServer : ServerBase<ServiceSession>
    {

        private ServiceProcessingQueue<ServiceSession> PacketProcessor;

        private static ServiceServer Instance { get; set; }
        
        public ServiceServer(int port) : base(port)
        {
            OnConnect += ServiceServer_OnConnect;
            OnDisconnect += ServiceServer_OnDisconnect;
            PacketProcessor = new ServiceProcessingQueue<ServiceSession>();
        }

        private void ServiceServer_OnDisconnect(object sender, SessionEventArgs<ServiceSession> e)
        {
            ServiceSessionManager.Instance.RemoveSession(e.Session);
            ServiceTitle.Update();
        }

        private void ServiceServer_OnConnect(object sender, SessionEventArgs<ServiceSession> e)
        {

            if (!ServiceSessionManager.Instance.AddSession(e.Session))
            {
                SocketLog.Write(SocketLogLevel.Warning, "Access Denied IP {0} Maxium of MatchServer Reached ", e.Session.ToString());
                e.Session.Close();
            }
            ServiceTitle.Update();
        }

        [InitializerMethod]
        public static bool initialize()
        {
            Instance = new ServiceServer(ServiceConfiguration.Instance.Service.Port);
            Instance.PacketProcessor.StartWorkerThreads(ServiceConfiguration.Instance.Service.NetworThreads);

            Instance.StartAccept();
            SocketLog.Write(SocketLogLevel.Debug, "Listening Service on port {0}", ServiceConfiguration.Instance.Service.Port);
            return true;
        }
        protected override ServiceSession CreateClient(Socket socket)
        {
            return new ServiceSession(socket);
        }

        protected override void ReceiveMessage(ServiceSession client, MemoryStream packet)
        {
            ServiceDataProccessingInfo<ServiceSession> n = new ServiceDataProccessingInfo<ServiceSession>(client, new ServicePacket());
            Console.WriteLine("recv");
            client.Socket.Send(System.Text.ASCIIEncoding.ASCII.GetBytes("klobros"));
        }
    }
}
