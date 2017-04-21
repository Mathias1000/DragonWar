using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public abstract class ServerBase<TSession>
        where TSession : SessionBase
    {
        private TcpListener m_listener;

        public event EventHandler<SessionEventArgs<TSession>> OnConnect;
        public event EventHandler<SessionEventArgs<TSession>> OnDisconnect;

        private void InvokeConnect(TSession mSession) => OnConnect?.Invoke(this, new SessionEventArgs<TSession>(mSession));
        private void InvokeDisconnect(TSession mSession) => OnDisconnect?.Invoke(this, new SessionEventArgs<TSession>(mSession));


        public ServerBase(int port)
        {
            IPAddress ip = IPAddress.Any;
            m_listener = new TcpListener(ip, port);
            OnDisconnect += ServerBase_OnDisconnect;
            OnConnect += ServerBase_OnConnect;
            
        }

        private void ServerBase_OnConnect(object sender, SessionEventArgs<TSession> e)
        {
            SocketLog.Write(SocketLogLevel.Debug, "Openned connection from " + e.Session.Socket.RemoteEndPoint);
        }

        private void ServerBase_OnDisconnect(object sender, SessionEventArgs<TSession> e)
        {
            SocketLog.Write(SocketLogLevel.Debug, "closed connection: {0}", e.Session.Socket.RemoteEndPoint);
        }

        public TcpListener TcpListener
        {
            get { return m_listener; }
        }

        public void StartAccept()
        {
            m_listener.Start();
            m_listener.BeginAcceptTcpClient(HandleAsyncConnection, m_listener);
        }

        private void HandleAsyncConnection(IAsyncResult res)
        {
            m_listener.BeginAcceptTcpClient(HandleAsyncConnection, m_listener);
            TcpClient client = m_listener.EndAcceptTcpClient(res);

            if (!IsClientAccepted(client.Client))
            {
                SocketLog.Write(SocketLogLevel.Warning, "Refused connection from " + client.Client.RemoteEndPoint);
                return;
            }


            ServerStateObject state = new ServerStateObject()
            {
                client = CreateClient(client.Client)
            };
            InvokeConnect(state.client);

            if (!state.client.IsClosing)
            {
                client.Client.BeginReceive(state.buffer, 0, ServerStateObject.BufferSize, SocketFlags.None, HandleAsyncReceive, state);
            }
        }

        private void HandleAsyncReceive(IAsyncResult res)
        {
            ServerStateObject state = (ServerStateObject)res.AsyncState;
            TSession Session = state.client;

            try
            {
                // read data from the client socket
                int read = Session.Socket.EndReceive(res);

                // data was read from client socket
                if (read > 0)
                {

                    using (BinaryPacket stream = new BinaryPacket(state.buffer))
                        ReceiveData(Session, stream);

                    if (!Session.IsClosing)
                    {
                        // begin receiving again after handling, so we can re-usse the same stateobject/buffer without issues.
                        Session.Socket.BeginReceive(state.buffer, 0, ServerStateObject.BufferSize, 0, HandleAsyncReceive, state);
                    }
                    else
                    {
                        InvokeDisconnect(Session);
                    }
                }
                else
                {
                    InvokeDisconnect(Session);
                    Session.Close();
                }
            }
            catch (SocketException e)
            {
                switch (e.SocketErrorCode)
                {
                    // TODO: More SocketError handling.
                    case SocketError.ConnectionReset:
                        InvokeDisconnect(Session);
                        Session.Close();
                        break;
                    default:
                        // Swallow the exception, but make sure a log is created and close the client.
                        SocketLog.Write(SocketLogLevel.Exception,
                                "Unhandled SocketException (Error:{0}) for Session {1} while receiving. Closing the Session.",
                            e.SocketErrorCode, Session.ToString());
                        InvokeDisconnect(Session);
                        Session.Close();
                        break;
                }
            }
            catch (Exception e)
            {
                // Note: State for this client may have been corrupted at this point, this is 
                //  an important issue when we reach this handler. This will force process to close.

                // Todo: Close anything else if needed (i.e. session, this will catch every unhandled exception in the handler, ect).
                SocketLog.Write(SocketLogLevel.Exception, "Exception occured while handling a packet. {0}\n{1}", e.Message, e.StackTrace);

                // We're most likely crashing, client is going to close anyway,
                //  maybe it's better not to do more with it.
                //client.Close();

                throw;
            }
        }

        protected virtual bool IsClientAccepted(Socket clientSocket)
        {
            return true;
        }

        protected abstract void ReceiveData(TSession client, BinaryPacket packet);
        protected abstract TSession CreateClient(Socket socket);
        // See: http://msdn.microsoft.com/en-us/library/5w7b7x5f.aspx
        private class ServerStateObject
        {
            //TODO MAKE ANOTHER
            public TSession client;
            // TODO/NOTE: This is the size used by client, should we use another?
            public const int BufferSize = 1024;//0x1FFFE;
            public byte[] buffer = new byte[BufferSize];
        }
    }
}
