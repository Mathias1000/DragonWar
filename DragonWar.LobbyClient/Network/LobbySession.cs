using DragonWar.Networking.Network;
using System.Net.Sockets;
using System.IO;
using DragonWar.Networking.Packet;
using System;
using DragonWar.Networking.Packet.Proccessing;

namespace DragonWar.LobbyClient.Network
{
    public class LobbySession : ServiceClient<LobbySession>
    {
      public LobbyProcessingQueue<LobbySession> PacketProcessor {  get; private set; }


        public LobbySession(Socket sock) : base(sock)
        {
            PacketProcessor = new LobbyProcessingQueue<LobbySession>();
        }
        protected override void ReceiveData(ServiceClient<LobbySession> client, BinaryPacket packet)
        {

            if (!packet.Read(out int Lenght) || !packet.ReadBytes(Lenght, out byte[] PacketData))
            {
                return;
            }
            dynamic Packet = PacketData.ToPacket<LobbyPacket>();

            PacketProcessor.EnqueueProcessingInfo(new LobbyProccessingInfo<LobbySession>(this, Packet));

        }

        public void SendPacket(LobbyPacket pPacket)
        {
            if (!pPacket.Read().GetType().IsSerializable)
            {
                SocketLog.Write(SocketLogLevel.Warning, "Packet {0} is not IsSerializable can not send", pPacket.GetType());
                return;
            }
            Send(pPacket.Write());
        }
    }
}
