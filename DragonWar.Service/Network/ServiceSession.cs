﻿using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonWar.Networking.Network.TCP;

namespace DragonWar.Service.Network
{
    public class ServiceSession : ServiceClientBase
    {
        public ServiceSession(Socket mSocket) : base(mSocket)
        {
        }

    }
}
