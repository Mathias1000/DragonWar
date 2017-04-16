using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace DragonWar.MatchServer.Network
{
    public class GameSession : GameSessionBase
    {
        public GameSession(Socket sock) : base(sock)
        {
        }
    }
}
