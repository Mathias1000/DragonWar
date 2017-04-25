using DragonWar.Networking.Packet.Service.Authentication;
using DragonWar.Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.InternNetwork.Handlers
{
    [ServiceHandlerClass(ServiceHeaderType.Protocol)]
    public class ProtocolHandler
    {
        public static void HandleServerHandshake(ServiceSession mSession, Auth_ACK mPacket)
        {
            
            if (mPacket.Password.Equals(ServiceConfiguration.Instance.Service.ServerPassword))
            {
                mSession.Authenticated = true;

                mSession.SendPacket(new Auth_RES
                {
                    IsOK = true,
                    SessionId = mSession.SessiondId,
                });
            }
            else
            {
                mSession.SendPacket(new Auth_RES
                {
                     IsOK = false,
                });
            }
        }
    }
}
