﻿using DragonWar.Networking.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet.Proccessing
{
   public class LobbyProccessingInfo<Session> : DataProcessingInfo<Session, LobbyPacket>  where Session : ServiceClient<Session>
    {
        public LobbyProccessingInfo(Session session, LobbyPacket pPacket) : base(session, pPacket)
        {
        }
    }
}
