using DragonWar.MatchServer.Config;
using DragonWar.MatchServer.Network;
using DragonWar.MatchServer.ServerConsole.Title;
using DragonWar.Networking.Packet.Lobby.Protocol;
using DragonWar.Networking.Packet.Service.Authentication;
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

            ServiceClient mClient = new ServiceClient(new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp));
            mClient.TryConnectToLogin("127.0.0.1", 900, 10);
            int i = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(3000);

                Auth_ACK mack = new Auth_ACK();
                mack.Password = i.ToString();
                mClient.SendPacket(mack);
                i++;
                if(i == 10)
                {
                    mClient.Close();
                }
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
