using DragonWar.Game.Server;
using DragonWar.MatchServer.Config;
using DragonWar.Networking.Packet.Service.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.MatchServer.InternNetwork.Handler
{
    [ServiceHandlerClass(ServiceHeaderType.Protocol)]
    public class ProtocolHandler
    {
        [ServiceHandler((byte)ConnectionTypes.Auth_SERVER_INFO)]
        public static void HandleServerHandshake(ServiceClient mSession, Auth_RES mPacket)
        {
            if(!mPacket.IsOK)
            {
                SocketLog.Write(SocketLogLevel.Exception, "Service Connection Refuse");
                return;
            }

            mSession.SendPacket(new Auth_SERVER_INFO
            {
                ServerInfo = new MatchServerInfo
                {
                    MaxPort = MatchServerConfiguration.Instance.MaxPort,
                    MinPort = MatchServerConfiguration.Instance.MinPort,
                    ServerIP = MatchServerConfiguration.Instance.GameIP,
                }

            });
        }
    }
}
