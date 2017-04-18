using System;
using System.Net.Sockets;
using System.IO;

using DragonWar.MatchServer.Config;
using DragonWar.Networking.Network;

namespace DragonWar.MatchServer.Network
{
    [ServerModule(ServerType.Match,InitializationStage.Networking)]
    public class ServiceSession : ServiceClient<ServiceSession>
    {
        public static ServiceSession Instance { get; set; }

        public ServiceSession(Socket sock) : base(sock)
        {
        }
        [InitializerMethod]
        public static bool Connect()
        {
            Instance = new ServiceSession(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));

            SocketLog.Write(SocketLogLevel.Startup, "Connect to Service on {0}:{1}!",MatchServerConfiguration.Instance.ConnectInfo.ConnectIP, MatchServerConfiguration.Instance.ConnectInfo.ConnectPort);
            Instance.TryConnectToLogin(MatchServerConfiguration.Instance.ConnectInfo.ConnectIP, MatchServerConfiguration.Instance.ConnectInfo.ConnectPort);

            if (Instance.Socket.Connected)
            {
                Instance.Start();
                return true;
            }

            return false;

        }
        protected void TryConnectToLogin(string host, int port, int tryCount = 0)
        {
            try
            {
                Socket.Connect(host, port);
            }
            catch (Exception e)
                when (tryCount >= 5)
            {
                // we already tried 5 time
                EngineLog.Write(EngineLogLevel.Exception, $"Failed to connect to Service after {tryCount} tries");
                EngineLog.Write(EngineLogLevel.Exception, "Could connect to Service server! Shutdown...", e);

            }
            catch   // if no "when"-clauses filter the exception out
            {
                // we haven't tried 5 times yet
                EngineLog.Write(EngineLogLevel.Exception, $"Try {tryCount} to connect to Service failed, trying again...");
                TryConnectToLogin(host, port, tryCount + 1);
            }
        }

        protected override void ReceiveMessage(ServiceClient<ServiceSession> client, MemoryStream packet)
        {
            client.Socket.Send(System.Text.ASCIIEncoding.ASCII.GetBytes("Test"));
            Console.WriteLine("m");
            System.Threading.Thread.Sleep(1000);
        }
    }
}
