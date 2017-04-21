using DragonWar.MatchServer.Config;
using DragonWar.MatchServer.Network;
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

            InternalInstance.LoadThreadPool(MatchServerConfiguration.Instance.WorkThreadCount);

            if (!InternalInstance.LoadGameServerModules())
            {
                throw new StartupException("Invalid Load Server");
            }
            ServiceSession.Instance.Socket.Send(System.Text.ASCIIEncoding.ASCII.GetBytes("lool"));
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
