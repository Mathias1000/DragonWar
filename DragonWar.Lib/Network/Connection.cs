using System;
using System.IO;
using DragonWar.Lib.Cryptography;
using DragonWar.Lib.Util;
using DragonWar.Lib.Packet;

namespace DragonWar.Lib.Network
{
    public class Connection : IDisposable
    {
        // Constants
        public const int ReceivingBufferSize = ushort.MaxValue * 2;
        public const int SendingBufferSize = ushort.MaxValue;


        // Public Properties
        public CryptoDirection CryptoOption { get; set; }
        public ICryptoProvider CryptoProvider { get; set; }
        public bool IsAlive { get; protected set; }

        // Events
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;
        public event EventHandler<PacketSentEventArgs> PacketSent;
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        // Protected Properties
        protected MultiArrayBuffer<byte> SendingBuffer;
        protected Stream DataSource;
        protected byte[] CurrentReceiveBuffer;
        protected int ReceiveSize;
        protected int CurrentPositionInReceiveBuffer;

        protected bool IsSendingData;

        // Private Properties
        private byte[] currentSendBuffer;
        private object readState;
        private object writeState;

        // Constructors
        protected Connection()
        {
            IsAlive = false;
            IsSendingData = false;
            SendingBuffer = new MultiArrayBuffer<byte>();
            CurrentReceiveBuffer = new byte[ReceivingBufferSize];
            ReceiveSize = 0;
            CurrentPositionInReceiveBuffer = 0;
            currentSendBuffer = new byte[SendingBufferSize];
            readState = new object();
            writeState = new object();
        }

        public Connection(Stream dataSource) : this()
        {
            this.DataSource = dataSource;
            IsAlive = true;
            BeginReadAndWriteLoops();
        }

        // Public Methods
        public void Close()
        {
            IsAlive = false;
        }

        public void SendPacket(PacketWriter pWriter)
        {
            if (!IsAlive)
                return;
            byte[] data;
            if (CryptoOption.HasFlag(CryptoDirection.Outgoing))
            {
                data = CryptoProvider.Encrypt(pWriter);
            }
            else
            {
                data = pWriter.GetBytes();
            }
            SendingBuffer.AppendBuffer(data, 0, data.Length);
            OnPacketSent(pWriter);
            if (!IsSendingData)
                StartSendingData();
        }

        // Protected Methods
        protected virtual void BeginReadAndWriteLoops()
        {
            StartStreamRead();
            StartSendingData();
        }

        protected virtual void StartStreamRead()
        {
            try
            {
                DataSource.BeginRead(
                    CurrentReceiveBuffer,
                    ReceiveSize,
                    CurrentReceiveBuffer.Length - ReceiveSize,
                    EndStreamRead,
                    null);
            }
            catch (ObjectDisposedException)
            {
                IsAlive = false;
                if (!IsSendingData)
                    OnDisconnected();
            }
        }

        private void EndStreamRead(IAsyncResult ar)
        {
            try
            {

                int bytesRead = DataSource.EndRead(ar);
                ReceiveSize += bytesRead;
                DataReceived(bytesRead);
                if (IsAlive)
                    StartStreamRead();
            }
            catch (IOException)
            {
                OnDisconnected();
            }
        }

        protected virtual void StartSendingData()
        {
            Console.WriteLine("Starting to send");
            IsSendingData = true;
            SendNextBuffer();
        }

        protected virtual void SendNextBuffer()
        {
            try
            {
                int bufferSize = Math.Min(SendingBufferSize, SendingBuffer.ElementsRemaining);
                if (bufferSize == 0)
                {
                    SendingBuffer.WaitForNewBuffers(TimeSpan.FromSeconds(1));
                    bufferSize = Math.Min(SendingBufferSize, SendingBuffer.ElementsRemaining);
                    if (bufferSize == 0)
                    {
                        IsSendingData = false;
                        return;
                    }
                }
                currentSendBuffer = SendingBuffer.ReadBuffer(bufferSize, true);
                DataSource.BeginWrite(
                    currentSendBuffer,
                    0,
                    currentSendBuffer.Length,
                    EndStreamWrite,
                    null);
            }
            catch (Exception e)
            {
                Console.WriteLine("error while sending data", e);
                IsAlive = false;
                IsSendingData = false;
                OnDisconnected();
            }
        }

        private void EndStreamWrite(IAsyncResult ar)
        {
            DataSource.EndWrite(ar);
            if (IsAlive)
                SendNextBuffer();
            else
                OnDisconnected();
        }

        protected virtual void OnPacketReceived(PacketReader pReader)
        {
            PacketReceived?.Invoke(this, new PacketReceivedEventArgs(pReader));
        }

        protected virtual void OnPacketSent(PacketWriter pWriter)
        {
            PacketSent?.Invoke(this, new PacketSentEventArgs(pWriter));
        }

        protected virtual void OnDisconnected()
        {
            Console.WriteLine("Client disconnected");
            IsAlive = false;
            DataSource.Close();
            Disconnected?.Invoke(this, new DisconnectedEventArgs());
        }

        protected virtual int GetSizeOfNextPacket()
        {
            if (CurrentReceiveBuffer[CurrentPositionInReceiveBuffer] != 0)
                return CurrentReceiveBuffer[CurrentPositionInReceiveBuffer];
            else
                return BitConverter.ToUInt16(CurrentReceiveBuffer, CurrentPositionInReceiveBuffer + 1);
        }

        protected int GetHeaderSize()
        {
            return CurrentReceiveBuffer[CurrentPositionInReceiveBuffer] != 0 ? 1 : 3;
        }

        protected void DataReceived(int bytesReceived)
        {
            while (CanReadNextPacket())
            {
                ReadNextPacket();
            }
        }

        protected bool CanReadNextPacket()
        {
            if (ReceiveSize == 0)
                return false;
            int headerSize = GetHeaderSize();
            if (ReceiveSize <= headerSize)
                return false;
            int bodySize = GetSizeOfNextPacket();
            int packetSize = headerSize + bodySize;
            return ReceiveSize >= packetSize;
        }

        protected void ReadNextPacket()
        {
            int headerSize = GetHeaderSize();
            int bodySize = GetSizeOfNextPacket();
            int packetSize = headerSize + bodySize;

            byte[] packetBody = new byte[bodySize];
            Buffer.BlockCopy(
                CurrentReceiveBuffer,
                CurrentPositionInReceiveBuffer + headerSize,
                packetBody,
                0,
                bodySize);
            if (CryptoOption.HasFlag(CryptoDirection.Incoming))
            {
                packetBody = CryptoProvider.Decrypt(packetBody, 0, packetBody.Length);
            }
            var packet = new DragonWarPacketReader(packetBody);
            Console.WriteLine("Read packet H{0} T{1}", packet.Header, packet.Type);

            OnPacketReceived(packet);

            ReceiveSize -= packetSize;
            CurrentPositionInReceiveBuffer += packetSize;
            // TODO: Check if this works
            // we try to advance the buffer here, to make place for new data.
            // This should be more performant then the "always-copy-after-packet" solution
            if (CurrentPositionInReceiveBuffer >= CurrentReceiveBuffer.Length / 2)
            {
                Buffer.BlockCopy(
                    CurrentReceiveBuffer,
                    CurrentPositionInReceiveBuffer,
                    CurrentReceiveBuffer,
                    0,
                    ReceiveSize);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).  
                    SendingBuffer.Dispose();
                    DataSource.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                CurrentReceiveBuffer = null;
                currentSendBuffer = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        // ~Connection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
