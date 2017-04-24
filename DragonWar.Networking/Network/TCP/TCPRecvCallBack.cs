using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP
{
    public class TCPRecvCallBack
    {
        public byte[] CurrentReceiveBuffer { get; private set; }


        public const int ReceivingBufferSize = ushort.MaxValue * 2;

        private Socket mSocket;

        public EventHandler<DataRecievedEventArgs> OnDataRecived;
        public event EventHandler<SocketDisconnectArgs> OnError;

        private void InvokeError(SocketError Error, string msg = "") => OnError?.Invoke(this, new SocketDisconnectArgs(Error, msg));

        public TCPRecvCallBack(Socket mSocket)
        {

            CurrentReceiveBuffer = new byte[ReceivingBufferSize];


            this.mSocket = mSocket;
        }

        public void Start()
        {
            BeginReceive();
        }

        private void BeginReceive()
        {
            if (!mSocket.Connected)
                return;

            try
            {
                var args = new SocketAsyncEventArgs();
                args.Completed += FinishReceive;
                args.SetBuffer(CurrentReceiveBuffer,
                    0,
                    CurrentReceiveBuffer.Length);

                if (!mSocket.ReceiveAsync(args))
                {
                    FinishReceive(this, args);
                }
            }
            catch (Exception ex)
            {
                InvokeError(SocketError.SocketError, $"Error beginning receive: {ex}");
            }
        }
        private void FinishReceive(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                var transfered = args.BytesTransferred;

                if (transfered < 1)
                {
                    InvokeError(args.SocketError);
                    return;
                }

                byte[] PacketData = new byte[transfered];

                Buffer.BlockCopy(CurrentReceiveBuffer, 0, PacketData, 0, transfered);
                using (var mPacket = new BinaryPacket(PacketData))
                {
                    OnDataRecived.Invoke(this, new DataRecievedEventArgs(mPacket));
                }

            }
            catch (Exception e)
            {
                InvokeError(SocketError.SocketError, e.ToString());
            }
            finally
            {
                args.Dispose();
                BeginReceive();
            }
        }

    }
}
