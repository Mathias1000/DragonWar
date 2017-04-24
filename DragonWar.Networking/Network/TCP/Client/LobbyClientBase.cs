using DragonWar.Networking.Network.Processing;
using DragonWar.Networking.Packet;
using DragonWar.Networking.Store;
using System;
using System.Net.Sockets;

namespace DragonWar.Networking.Network.TCP.Client
{
    public class LobbyClientBase : ClientBase
    {
        public event Action<DataProcessingInfo<LobbyClientBase,LobbyPacket>> NewProcessingInfo;

        public LobbyClientBase(Socket mSocket) : base(mSocket)
        {

        }


        protected override void OnDataRecv(object sender, DataRecievedEventArgs m)
        {
            if (!m.CurrentDataPacket.Read(out int Lenght) || !m.CurrentDataPacket.ReadBytes(Lenght, out byte[] PacketData))
            {
                return;
            }

            dynamic Packet = PacketData.ToPacket<LobbyPacket>();

            if (NewProcessingInfo == null)
            {
                SocketLog.Write(SocketLogLevel.Debug, "NewProcessingInfo not set, handling the Lobbypacket directly");
                HandlePacket(Packet);
            }
            else
            {
                OnNewProcessingInfo(Packet);
            }
        }


        protected virtual void OnNewProcessingInfo(LobbyPacket packet)
        {
            NewProcessingInfo?.Invoke(new DataProcessingInfo<LobbyClientBase, LobbyPacket>(this, packet));
        }

        public override void HandlePacket<T>(T Packet)
        {
            LobbyHandlerStore.Instance.HandlePacket(Packet as dynamic, this);
        }

        protected virtual void SendPacket(ServicePacket pPacket)
        {
            if (!pPacket.Read().GetType().IsSerializable)
            {
                SocketLog.Write(SocketLogLevel.Warning, "LobbyPacket {0} is not IsSerializable can not send", pPacket.GetType());
                return;
            }

            Send(pPacket.Write());
        }
    }
}
