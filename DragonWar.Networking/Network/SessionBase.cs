using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public abstract class SessionBase
    {
        private Socket m_socket;

        public bool IsClosing { get; private set; } = true;

        public Socket Socket
        {
            get { return m_socket; }
            protected set { m_socket = value; }
        }

        public SessionBase(Socket sock)
        {
            Socket = sock;
            IsClosing = false;
        }

        public void Close()
        {
            IsClosing = true;
            Socket.ShutdownSafely();
        }


        public void SendMessage(MemoryStream stream)
        {
            // we're using the stream's buffer, is this thread-safe for whatever may be re-using the buffer after?
            byte[] buffer = stream.GetBuffer();
            Socket.BeginSend(buffer, 0, (int)stream.Length, SocketFlags.None, new AsyncCallback(HandleAsyncSend), this);
        }

        private static void HandleAsyncSend(IAsyncResult res)
        {
            SessionBase client = (SessionBase)res.AsyncState;

            try
            {
                int bytesSent = client.Socket.EndSend(res);
            }
            catch (SocketException e)
            {
                // Swallow the exception, but make sure a log is created and close the client.
               SocketLog.Write(SocketLogLevel.Exception,
                    "Unhandled SocketException (Error:{0}) for Session {1} while sending. Closing the Session.",
                    e.SocketErrorCode, client);
                client.Close();
            }
            catch (Exception e)
            {
                SocketLog.Write(SocketLogLevel.Exception, "Failed to send message to a client. Exception: {0}\n{1}", e.Message, e.Source);

                // don't swallow, this is a bug.
                throw;
            }
        }

        public override string ToString()
        {
            if (Socket == null)
                return "Invalid SessionBase";

            return Socket.RemoteEndPoint.ToString();
        }
    }
}
