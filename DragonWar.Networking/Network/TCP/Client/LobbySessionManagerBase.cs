using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP.Client
{
    public class LobbySessionManagerBase<TClient> : SessionManagerBase<TClient> where TClient : LobbyClientBase
    {
        public LobbySessionManagerBase(ushort MaxSessions) : base(MaxSessions)
        {
        }
    }
}
