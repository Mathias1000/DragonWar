using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP.Client
{
    public abstract class ClientBase
    {
        private TCPRecvCallBack RecvCallBack;
        private TCPSendCallBack SendCallBack;

        private Socket mSocket;


        public ClientBase(Socket mSocket)
        {
            this.mSocket = mSocket;
            RecvCallBack = new TCPRecvCallBack(mSocket);
            SendCallBack = new TCPSendCallBack(mSocket);

            //Events
            RecvCallBack.OnDataRecived += OnDataRecv;
            RecvCallBack.OnError += HandleSocketError;
            SendCallBack.OnSendError += HandleSocketError;
        }

        private void HandleSocketError(object sender, SocketDisconnectArgs e)
        {
            switch(e.Error)
            {
                case SocketError.Success://DC By Recive
                case SocketError.ConnectionReset:
                    SocketLog.Write(SocketLogLevel.Debug, $"Closed connection: { mSocket.RemoteEndPoint }");
                    break;
                default:
                    SocketLog.Write(SocketLogLevel.Warning, $"Unhandle SocketError : {e.Error} : message : {e.Message}");
                    break;
                case SocketError.SocketError:
                    SocketLog.Write(SocketLogLevel.Exception, $"Unkown SocketError {e.Message}");
                    break;
            }

            this.mSocket.ShutdownSafely();
       
        }

        public virtual void Close() => mSocket.ShutdownSafely();

        public virtual void StartRecv() => RecvCallBack.Start();

        protected virtual void Send(byte[] data) => SendCallBack.Send(data);

        protected abstract void OnDataRecv(object sender, DataRecievedEventArgs m);

        public abstract void HandlePacket<T>(T Packet);

        public void TryConnectToLogin(string host, int port, int tryCount = 0)
        {
            try
            {
                EngineLog.Write(EngineLogLevel.Startup, $"Connect To {host}:{port}");
                mSocket.Connect(host, port);
            }
            catch (Exception e)
                when (tryCount >= 5)
            {
                // we already tried 5 time
                EngineLog.Write(EngineLogLevel.Exception, $"Failed to connect to  after {tryCount} tries");
                EngineLog.Write(EngineLogLevel.Exception, "Could connect to server! Shutdown...", e);

            }
            catch   // if no "when"-clauses filter the exception out
            {
                // we haven't tried 5 times yet
                EngineLog.Write(EngineLogLevel.Exception, $"Try {tryCount} to connect to failed, trying again...");
                TryConnectToLogin(host, port, tryCount + 1);
            }
        }
    }
}
