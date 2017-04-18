using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public class ServiceSessionBase : SessionBase
    {

        public ushort SessiondId { get; set; }

        public ServiceSessionBase(Socket sock)
             : base(sock)
        {
            if (sock == null)
                throw new ArgumentNullException();
        }

    }
}
