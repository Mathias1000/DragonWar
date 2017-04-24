using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP.Server
{
    public class ServerBase
    {
        TcpListener listener;
        bool isRunning;

        public ServerBase(string ip, int port)
        {
            var bindIP = IPAddress.None;

            if (!IPAddress.TryParse(ip, out bindIP))
            {
                SocketLog.Write(SocketLogLevel.Exception, $"Server can't be started: Invalid IP-Address ({ip})");
                return;
            }

            try
            {
                listener = new TcpListener(bindIP, port);
                listener.Start();
                SocketLog.Write(SocketLogLevel.Startup, "Listening on {0}", listener.LocalEndpoint);
                if (isRunning = listener.Server.IsBound)
                    new Thread(AcceptConnection).Start(5);
            }
            catch (Exception ex)
            {
                SocketLog.Write(SocketLogLevel.Exception, ex.ToString());
            }
        }
        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();


            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }


            return inUse;
        }
        async void AcceptConnection(object delay)
        {
            while (isRunning)
            {
                await Task.Delay((int)delay);

                if (listener.Pending())
                {
                    var clientSocket = listener.AcceptSocket();

                    if (clientSocket != null)
                    {
                        SocketLog.Write(SocketLogLevel.Debug, $"Openned connection from { clientSocket.RemoteEndPoint }");
                        await DoWork(clientSocket);
                    }
                }
            }
        }

        public virtual Task DoWork(Socket client)
        {
            return null;
        }

        public void Dispose()
        {
            listener = null;
            isRunning = false;
        }
    }
}
