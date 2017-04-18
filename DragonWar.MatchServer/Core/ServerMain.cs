using DragonWar.MatchServer.Config;
using DragonWar.MatchServer.ServerConsole.Title;
using DragonWar.Utils.Core;


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
