using DragonWar.Networking.Network;

using System.Net.Sockets;

namespace DragonWar.Service.Network
{
    public class ServiceSession : ServiceSessionBase
    {
        public ServiceSession(Socket sock) : base(sock)
        {
        }
    }
}
