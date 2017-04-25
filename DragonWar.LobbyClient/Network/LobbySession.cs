using System.Net.Sockets;
using DragonWar.Networking.Network.TCP.Client;
using DragonWar.Networking.Network.Processing;
using DragonWar.Networking.Packet;

namespace DragonWar.LobbyClient.Network
{
    
    public class LobbySession : LobbyClientBase
    {
        private DataProcessingQueue<LobbyClientBase, LobbyPacket> ProcessingQueue { get; set; }

        public LobbySession(Socket mSocket) : base(mSocket)
        {
            ProcessingQueue = new DataProcessingQueue<LobbyClientBase, LobbyPacket>();
            ProcessingQueue.Start();
            NewProcessingInfo +=
            info => ProcessingQueue.EnqueueProcessingInfo(info);
        }
   
    }
}
