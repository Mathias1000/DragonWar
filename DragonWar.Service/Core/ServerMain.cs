using DragonWar.Service.Config;
using DragonWar.Service.Network;
using DragonWar.Service.ServerConsole.Title;
using DragonWar.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.Core
{
    public class ServerMain : ServerMainBase
    {
        public static new ServerMain InternalInstance { get; private set; }


        public ServerMain() : base(ServerType.Service)
        {
          //  ServiceTitle.Update();
        }
        public override void Shutdown()
        {
            base.Shutdown();

            ThreadPool.Dispose();
            DB.Dispose();
        }

        public override bool LoadBaseServerModule()
        {
            return base.LoadBaseServerModule();
        }

        public static bool Initialize()
        {
            InternalInstance = new ServerMain();
            InternalInstance.WriteConsoleLogo();

            if (!ServiceConfiguration.Initialize())
            {
                throw new StartupException("Invalid Load ServiceConfiguration");
            }



            if (!InternalInstance.LoadBaseServerModule())
            {
                throw new StartupException("Invalid Load Server");
            }


            InternalInstance.LoadThreadPool(ServiceConfiguration.Instance.WorkThreadCount);



            if (!DB.Start(ServiceConfiguration.Instance.DatabaseInfo))
            {
                throw new StartupException("Invalid Load DatabaseManager");
            }

            if (!InternalInstance.LoadGameServerModules())
            {
                throw new StartupException("Invalid Load Server");
            }

            InternalInstance.ServerIsReady = true;

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
