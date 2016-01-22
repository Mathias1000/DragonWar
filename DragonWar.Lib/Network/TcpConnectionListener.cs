using System;
using System.Net;
using System.Net.Sockets;

namespace DragonWar.Lib.Network
{
    public class TcpConnectionListener : ConnectionListener
    {

        private TcpListener _listener;
        private object _listenerLock;

        public TcpConnectionListener(IPEndPoint endPoint)
        {
            _listenerLock = new object();
            _listener = new TcpListener(endPoint);
        }

        public override void StartListening()
        {
            _listener.Start();
            _listener.BeginAcceptTcpClient(EndAcceptTcpClient, _listenerLock);
        }

        public override void StopListening()
        {
            _listener.Stop();
        }

        protected virtual void EndAcceptTcpClient(IAsyncResult ar)
        {
            TcpClient client = _listener.EndAcceptTcpClient(ar);
            _listener.BeginAcceptTcpClient(EndAcceptTcpClient, _listenerLock);
            Connection c = new Connection(client.GetStream());
            OnNewConnection(c);
        }
    }
}
