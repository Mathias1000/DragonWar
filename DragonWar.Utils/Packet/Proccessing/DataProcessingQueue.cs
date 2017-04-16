using DragonWar.Utils.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DragonWar.Utils.Packet
{
    public class DataProcessingQueue<SessionType, PacketType> where SessionType : SessionBase //: IDisposable
    {
        private BlockingCollection<DataProcessingInfo<SessionType, PacketType>> _blockingQueue;
        private List<Thread> _workerThreads;
        private int lastAmountOfWorkerThreads = 1;

        public DataProcessingQueue()
        {
            _blockingQueue = new BlockingCollection<DataProcessingInfo<SessionType, PacketType>>();
            _workerThreads = new List<Thread>();
        }

        public void StartWorkerThreads(int count)
        {

            SocketLog.Write(SocketLogLevel.Startup, "Starting {0} worker threads", count);
            for (int i = 0; i < count; i++)
                StartWorkerThread(string.Format("WRKR{0}", i));
            lastAmountOfWorkerThreads = count;
        }

        protected void StartWorkerThread(string name)
        {
            Thread t = new Thread(ProcessingLoop)
            {
                IsBackground = true,
                Name = name,
            };
            t.Start();
            _workerThreads.Add(t);
        }

        public void EnqueueProcessingInfo(DataProcessingInfo<SessionType, PacketType> info)
        {
            _blockingQueue.Add(info);
        }

        protected void ProcessingLoop()
        {
            try
            {
                while (!_blockingQueue.IsCompleted)
                {
                    DataProcessingInfo<SessionType, PacketType> info = _blockingQueue.Take();
                    ProcessInfo(info);
                }
            }
            catch (InvalidOperationException)
            {
                SocketLog.Write(SocketLogLevel.Startup, "no more work to do, shutting thread down");
            }
        }

        protected void ProcessInfo(DataProcessingInfo<SessionType, PacketType> info)
        {
            Console.WriteLine("Handle Packet");
        
        }

        public void Stop()
        {
            _blockingQueue.CompleteAdding();
        }

        public void Start()
        {
            if (_blockingQueue.IsAddingCompleted)
                _blockingQueue = new BlockingCollection<DataProcessingInfo<SessionType, PacketType>>();
            StartWorkerThreads(lastAmountOfWorkerThreads);
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
                    _blockingQueue.CompleteAdding();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        // ~FiestaProcessingQueue() {
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
