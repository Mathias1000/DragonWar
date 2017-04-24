using DragonWar.Networking.Handling.Store;
using DragonWar.Networking.Network.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP.Client
{
    public class ServiceClientBase : ClientBase
    {
        public event Action<DataProcessingInfo<ServiceClientBase,ServicePacket>> NewProcessingInfo;

        public ServiceClientBase(Socket mSocket) : base(mSocket)
        {
    
        }

        protected override void OnDataRecv(object sender, DataRecievedEventArgs m)
        {
            if (!m.CurrentDataPacket.Read(out int Lenght) || !m.CurrentDataPacket.ReadBytes(Lenght, out byte[] PacketData))
            {
                return;
            }

            dynamic Packet = PacketData.ToPacket<ServicePacket>();

            if (NewProcessingInfo == null)
            {
                SocketLog.Write(SocketLogLevel.Debug, "NewProcessingInfo not set, handling the ServicePacket directly");
                HandlePacket(Packet);
            }
            else
            {
                OnNewProcessingInfo(Packet);
            }
        }

        protected void OnNewProcessingInfo(ServicePacket packet)
        {
            NewProcessingInfo?.Invoke(new DataProcessingInfo<ServiceClientBase, ServicePacket>(this, packet));
        }

        public override void HandlePacket<T>(T Packet) 
        {
            ServiceHandlerStore.Instance.HandlePacket(Packet as dynamic, this);
        }
 

        public virtual void SendPacket(ServicePacket pPacket)
        {
            if (!pPacket.Read().GetType().IsSerializable)
            {
                SocketLog.Write(SocketLogLevel.Warning, "ServicePacket {0} is not IsSerializable can not send", pPacket.GetType());
                return;
            }

            Send(pPacket.Write());
        }
    }
}
