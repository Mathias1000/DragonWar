using System;
using System.IO;
using System.Net.Sockets;

using DragonWar.Service.Config;
using DragonWar.Service.ServerConsole.Title;
using DragonWar.Networking.Network;
using DragonWar.Networking.Packet;
using DragonWar.Networking.Handling.Store;

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
            ServiceHandlerStore.Instance.UnknownPacket += Instance_UnknownPacket;
        }

        private void Instance_UnknownPacket(ServicePacket reader)
        {
            Console.Write("Unk");
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
        public static bool Initialize()
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

        protected override void ReceiveData(ServiceSession client, BinaryPacket packet)
        {
            if (!packet.Read(out int Lenght) || !packet.ReadBytes(Lenght, out byte[] PacketData))
            {
                return;
            }

            dynamic Packet = PacketData.ToPacket<ServicePacket>();
            PacketProcessor.EnqueueProcessingInfo(new ServiceDataProccessingInfo<ServiceSession>(client, Packet));

        }
    }
}
