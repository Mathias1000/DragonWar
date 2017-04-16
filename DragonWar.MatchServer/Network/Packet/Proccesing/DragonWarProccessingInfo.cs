using DragonWar.Utils.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.MatchServer.Network.Packet.Proccesing
{
    public class DragonWarProccessingInfo : DataProcessingInfo<GameSession, GamePacket>
    {
        public DragonWarProccessingInfo(GameSession session, GamePacket pPacket) : base(session, pPacket)
        {
        }
    }
}
