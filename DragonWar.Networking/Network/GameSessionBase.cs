using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public class GameSessionBase : SessionBase
    {
        public GameSessionBase(Socket sock) : base(sock)
        {
        }
    }
}
