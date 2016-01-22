using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DragonWar.MatchServer
{
    public class MatchManager
    {
        public Dictionary<int, Match> MatchesByMatchID;
        public Dictionary<int, Match> MatchesBySummonerID;
       
    }
}
