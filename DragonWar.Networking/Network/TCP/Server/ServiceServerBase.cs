using DragonWar.Networking.Network.Processing;
using DragonWar.Networking.Network.TCP.Client;

namespace DragonWar.Networking.Network.TCP.Server
{
    public class ServiceServerBase : ServerBase
    {
        public DataProcessingQueue<ServiceClientBase, ServicePacket> ProcessingQueue { get; set; }

        public int WorkCount = 0;

        public ServiceServerBase(string ip, int port,int WorkCount) : base(ip, port)
        {
            ProcessingQueue = new DataProcessingQueue<ServiceClientBase, ServicePacket>();
            this.WorkCount = WorkCount;
        }

        public virtual void Start()
        {
            ProcessingQueue.StartWorkerThreads(WorkCount);
        }
        public virtual void Stop()
        {
            ProcessingQueue.Stop();
        }
    }
}
