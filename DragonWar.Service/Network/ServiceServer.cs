using DragonWar.Networking.Network.TCP.Server;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace DragonWar.Service.Network
{
    public class ServiceServer : ServiceServerBase
    {


        public ServiceServer(string ip, int port,int WorkCount) : base(ip, port,WorkCount)
        {
        }

        public override async Task DoWork(Socket client)
        {
            var mSession = new ServiceSession(client);

             mSession.NewProcessingInfo +=  
                info => ProcessingQueue.EnqueueProcessingInfo(info);

            await Task.Factory.StartNew(mSession.StartRecv);
        } 
    }
}
