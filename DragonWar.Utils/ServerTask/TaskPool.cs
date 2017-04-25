using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DragonWar.Utils.Core;
using System.Collections.Concurrent;

namespace DragonWar.Utils.ServerTask
{
    public sealed class TaskPool : IDisposable
    {

        private readonly List<Thread> _workers; // queue of worker threads ready to process actions
        private readonly BlockingCollection<IServerTask> _tasks;

        public TaskPool(int size)
        {

            this._workers = new List<Thread>();
            _tasks = new BlockingCollection<IServerTask>();

            for (var i = 0; i < size; ++i)
            {
                var worker = new Thread(this.Worker) { Name = string.Concat("Worker ", i) };
                worker.Start();
                _workers.Add(worker);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _tasks.CompleteAdding();
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


        public void QueueTask(IServerTask task)
        {
            _tasks.Add(task);
        }
        async Task<bool> UpdateServerTask(IServerTask task,GameTime TimeNow)
        {
            if (await Task.FromResult(task.Update(TimeNow)))
            {

                task.LastUpdate = TimeNow;

                return true;
            }
            return false;
        }
    
        async Task<bool> UpdateTaskMain(IServerTask task, DateTime lastUpdate)
        {
            try
            {
                var now = DateTime.Now;
                var elapsed = (now - lastUpdate);
                lastUpdate = now;


                GameTime TimeNow = new GameTime(now, elapsed, ServerMainBase.InternalInstance.TotalUpTime);

                ServerMainBase.InternalInstance.TotalUpTime += elapsed;
                ServerMainBase.InternalInstance.CurrentTime = TimeNow;


                if (TimeNow.Subtract(task.LastUpdate).TotalMilliseconds >= (int)task.Intervall)
                {
                        bool Res = await UpdateServerTask(task, TimeNow);

                        if (!Res)
                        {
                            task.InvokeOnLeave(TimeNow);
                            task.Dispose();
                            return false;
                        }
                        return true;
                }
                else
                {
                    return true;
                }

            }
            catch(Exception ex)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Remove Task {0}|{1}", task.GetType(), ex.ToString());
                return false;
            }
            
        }
        private async void Worker()
        {
            try
            {
                while (!_tasks.IsCompleted)
                {
                    IServerTask mTask = _tasks.Take();
                    if (await UpdateTaskMain(mTask, DateTime.Now))
                    {
                        _tasks.Add(mTask);

                        await Task.Delay(50);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                SocketLog.Write(SocketLogLevel.Startup, "no more work to do, shutting thread down");
            }
        }
    }
}
