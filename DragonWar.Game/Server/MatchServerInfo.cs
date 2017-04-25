using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Game.Server
{
    [Serializable]
    public class MatchServerInfo
    {
        public int SessionId { get; set; }

        public string ServerIP { get; set; }

        public int MinPort { get; set; }

        public int MaxPort { get; set; }

        [NonSerialized]
        private ConcurrentQueue<int> Ports;

        public MatchServerInfo(int MinPort,int MaxPort)
        {
            Ports = new ConcurrentQueue<int>();
            Ports.Fill(MinPort, MaxPort);

        }

        public bool TakePort(int Port) => Ports.TryDequeue(out Port);

        public void FreePort(int Port) => Ports.Enqueue(Port);

    }
}
