using DragonWar.Networking.Network;
using DragonWar.Networking.Packet;
using DragonWar.Networking.Packet.Lobby.Protocol;
using DragonWar.Networking.Packet.Lobby.Server;
using DragonWar.Networking.Packet.Proccessing;
using DragonWar.Service.Config;

using System;
using System.IO;
using System.Net.Sockets;

namespace DragonWar.Service.Network
{

    //Recive from LobbyClient
    [GameServerModule(ServerType.Service, GameInitalStage.Network)]
    public class LobbyServer : ServerBase<LobbySession>
    {
        private static LobbyServer Instance { get; set; }

        private LobbyProcessingQueue<LobbySession> PacketProcessor;

        public LobbyServer(int port) : base(port)
        {
            OnConnect += LobbyServer_OnConnect;
            OnDisconnect += LobbyServer_OnDisconnect;

            PacketProcessor = new LobbyProcessingQueue<LobbySession>();
        }

        private void LobbyServer_OnDisconnect(object sender, SessionEventArgs<LobbySession> e)
        {
            if(!LobbySessionManager.Instance.RemoveSession(e.Session.SessiondId))
            {
                //WTF SHIT
            }
        }

        private void LobbyServer_OnConnect(object sender, SessionEventArgs<LobbySession> e)
        {
            if (!LobbySessionManager.Instance.AddSession(e.Session))
            {

                ServerStatusPacket mPacket = new ServerStatusPacket
                {
                      State = ServerState.Full,
                };
                e.Session.SendPacket(mPacket);
                e.Session.Close();
            }
            else
            {
                LobbyHandShake HandShake = new LobbyHandShake
                {
                     
                };
                e.Session.EncryptKey = HandShake.EncrypPos;
                e.Session.SendPacket(HandShake);
            }
        
        }

        [InitializerMethod]
        public static bool Initialize()
        {
            Instance = new LobbyServer(ServiceConfiguration.Instance.GameServer.Port);
            Instance.PacketProcessor.StartWorkerThreads(ServiceConfiguration.Instance.GameServer.NetworThreads);

            Instance.StartAccept();

            SocketLog.Write(SocketLogLevel.Debug, "Listening LobbyServer on port {0}", ServiceConfiguration.Instance.GameServer.Port);

            return true;
        }
        protected override LobbySession CreateClient(Socket socket)
        {
            return new LobbySession(socket);
        }

        protected override void ReceiveData(LobbySession client,BinaryPacket packet)
        {
          //  PacketProcessor.EnqueueProcessingInfo(new DataProcessingInfo<ServiceSession, ServicePacket>(client, new LobbyPacket());
            Console.WriteLine("recv");
            client.Socket.Send(System.Text.ASCIIEncoding.ASCII.GetBytes("klobros"));
        }
    }
}
