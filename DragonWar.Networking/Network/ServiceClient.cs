using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network
{
    public abstract class ServiceClient<Client> : ServiceSessionBase where Client : ServiceSessionBase
    {
        public const int BufferSize = 1024;//0x1FFFE;

    
        ClientStateObject State { get; set; }

        public ServiceClient(Socket sock) : base(sock)
        {
  
        }

        private void Disconnect()
        {
            SocketLog.Write(SocketLogLevel.Debug, "closed connection from : {0}", Socket.RemoteEndPoint);
            Close();
        }

        public void Start()
        {
            State = new ClientStateObject();
            Socket.BeginReceive(State.buffer, 0, BufferSize, SocketFlags.None, HandleAsyncReceive, State);
        }


        public void HandleAsyncReceive(IAsyncResult res)
        {
          State = (ClientStateObject)res.AsyncState;

            try
            {
                // read data from the client socket
                int read = Socket.EndReceive(res);

                // data was read from client socket
                if (read > 0)
                {
                    using (BinaryPacket stream = new BinaryPacket(State.buffer))
                        ReceiveData(this, stream);

                    if (!IsClosing)
                    {
                        // begin receiving again after handling, so we can re-usse the same stateobject/buffer without issues.
                        Socket.BeginReceive(State.buffer, 0, BufferSize, 0, HandleAsyncReceive, State);
                    }
                    else
                    {
                        Disconnect(); 
                    }
                }
                else
                {
                    Disconnect();
                   
                }
            }
            catch (SocketException e)
            {
                switch (e.SocketErrorCode)
                {
                    // TODO: More SocketError handling.
                    case SocketError.ConnectionReset:
                        Disconnect();
                        break;
                    default:
                        // Swallow the exception, but make sure a log is created and close the client.
                        SocketLog.Write(SocketLogLevel.Exception,
                                "Unhandled SocketException (Error:{0}) for Session {1} while receiving. Closing the Session.",
                            e.SocketErrorCode, this.ToString());
                        Disconnect();
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

    
        protected abstract void ReceiveData(ServiceClient<Client> client, BinaryPacket packet);


        private class ClientStateObject
        {
            // TODO/NOTE: This is the size used by client, should we use another?
            public byte[] buffer = new byte[BufferSize];
        }
    }
}
