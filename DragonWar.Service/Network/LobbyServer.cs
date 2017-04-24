using DragonWar.Networking.Network.TCP.Server;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DragonWar.Service.Network
{
    public class LobbyServer : LobbyServerBase<LobbySession>
    {
        public LobbyServer(string ip, int port,int WorkCount) : base(ip, port,WorkCount)
        {
        }

        public override async Task DoWork(Socket client)
        {
            var mSession = new LobbySession(client);

            mSession.NewProcessingInfo +=
                info => ProcessingQueue.EnqueueProcessingInfo(info); 

            await Task.Factory.StartNew(mSession.StartRecv);
        }


    }
}
