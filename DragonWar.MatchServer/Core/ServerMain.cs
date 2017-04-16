using DragonWar.MatchServer.Config;
using DragonWar.MatchServer.Network;
using DragonWar.MatchServer.ServerConsole.Title;
using DragonWar.Utils.Core;
using DragonWar.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.MatchServer.Core
{
    public class ServerMain : ServerMainBase
    {
        public static new ServerMain InternalInstance { get; private set; }

        public ServerMain() : base(ServerType.Match)
        {
            MatchServerTitle.Update();
           
        }

        public override void Shutdown()
        {
            base.Shutdown();

            ThreadPool.Dispose();
           
        }

        public static bool Initialize()
        {
            InternalInstance = new ServerMain();
            InternalInstance.WriteConsoleLogo();

            if (!MatchServerConfiguration.Initialize())
            {
                throw new StartupException("Invalid Load MatchServerConfiguration");
            }
            //Hmm Need DB?
            if (!InternalInstance.LoadBaseServerModule())
            {
                throw new StartupException("Invalid Load Server");
            }

            InternalInstance.LoadThreadPool(MatchServerConfiguration.Instance.MatchServerInfo.NetworThreads);

            if (!InternalInstance.LoadGameServerModules())
            {
                throw new StartupException("Invalid Load Server");
            }

            return true;
        }

        static void Main(string[] args)
        {
            if (!ServerMain.Initialize())
            {

                ServerMain.InternalInstance.Shutdown();
            }
        }
    }
}
