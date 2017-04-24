using System;
using System.Net.Sockets;
using System.IO;

using DragonWar.MatchServer.Config;
using DragonWar.Networking.Network;
using DragonWar.Networking.Packet;
using DragonWar.Networking.Handling.Store;

namespace DragonWar.MatchServer.Network
{
    /*
    [ServerModule(ServerType.Match, InitializationStage.Networking)]
    public class ServiceSession : ServiceClient<ServiceSession>
    {
        public static ServiceSession Instance { get; set; }

        private ServiceProcessingQueue<ServiceSession> PacketProcessor { get; set; }

        public ServiceSession(Socket sock) : base(sock)
        {
            PacketProcessor = new ServiceProcessingQueue<ServiceSession>();
            ServiceHandlerStore.Instance.UnknownPacket += Instance_UnknownPacket;
        }

        private void Instance_UnknownPacket(ServicePacket reader)
        {
            SocketLog.Write(SocketLogLevel.Warning, $"Unknown ServicePacket H{(byte)reader?.Header} T{reader?.Handling}");
            SocketLog.Write(SocketLogLevel.Warning, reader.GetType().ToString());
        }

        [InitializerMethod]
        public static bool Connect()
        {
            Instance = new ServiceSession(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));

            Instance.PacketProcessor.StartWorkerThreads(1);

            SocketLog.Write(SocketLogLevel.Startup, "Connect to Service on {0}:{1}!", MatchServerConfiguration.Instance.ConnectInfo.ConnectIP, MatchServerConfiguration.Instance.ConnectInfo.ConnectPort);
            Instance.TryConnectToLogin(MatchServerConfiguration.Instance.ConnectInfo.ConnectIP, MatchServerConfiguration.Instance.ConnectInfo.ConnectPort);

            if (Instance.Socket.Connected)
            {
                Instance.Start();
                return true;
            }

         
            return false;

        }

        protected override void ReceiveData(ServiceClient<ServiceSession> client, BinaryPacket packet)
        {


            if (!packet.Read(out int Lenght) || !packet.ReadBytes(Lenght, out byte[] PacketData))
            {
                return;
            }
            dynamic Packet = PacketData.ToPacket<ServicePacket>();
            PacketProcessor.EnqueueProcessingInfo(new ServiceDataProccessingInfo<ServiceSession>(this, Packet));

        }*/
   // }
}
