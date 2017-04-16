using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DragonWar.Utils.Core;

namespace DragonWar.Utils.ServerTask
{
    public sealed class TaskPool : IDisposable
    {

        private readonly LinkedList<Thread> _workers; // queue of worker threads ready to process actions
        private readonly LinkedList<IServerTask> _tasks = new LinkedList<IServerTask>();
        private bool _disallowAdd; // set to true when disposing queue but there are still tasks pending
        private bool _disposed; // set to true when disposing queue and no more tasks are pending

        public TaskPool(int size)
        {
            this._workers = new LinkedList<Thread>();
            for (var i = 0; i < size; ++i)
            {
                var worker = new Thread(this.Worker) { Name = string.Concat("Worker ", i) };
                worker.Start();
                this._workers.AddLast(worker);
            }
        }

        public void Dispose()
        {
            var waitForThreads = false;
            lock (this._tasks)
            {
                if (!this._disposed)
                {
                    GC.SuppressFinalize(this);

                    this._disallowAdd = true; // wait for all tasks to finish processing while not allowing any more new tasks
                    while (this._tasks.Count > 0)
                    {
                        Monitor.Wait(this._tasks);
                    }

                    this._disposed = true;
                    Monitor.PulseAll(this._tasks); // wake all workers (none of them will be active at this point; disposed flag will cause then to finish so that we can join them)
                    waitForThreads = true;
                }
            }
            if (waitForThreads)
            {

                foreach (var worker in this._workers)
                {
                    worker.Join();
                }
            }
        }

        public void QueueTask(IServerTask task)
        {
            lock (_tasks)
            {
                if (_disallowAdd)
                {
                    throw new InvalidOperationException(
                        "This Pool instance is in the process of being disposed, can't add anymore");
                }
                if (_disposed)
                {
                    throw new ObjectDisposedException("This Pool instance has already been disposed");
                }
                task.InvokeOnEnter((GameTime)DateTime.Now);
                _tasks.AddLast(task);
                Monitor.PulseAll(_tasks); // pulse because tasks count changed
            }
        }
        bool UpdateServerTask(IServerTask task, GameTime TimeNow)
        {
            if (task.Update(TimeNow))
            {

                task.LastUpdate = TimeNow;

                return (true);
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

                int mm = (int)TimeNow.Subtract(task.LastUpdate).TotalMilliseconds;
                if (TimeNow.Subtract(task.LastUpdate).TotalMilliseconds >= (int)task.Intervall)
                {
                        bool Res = await Task.Run(() => UpdateServerTask(task, TimeNow));

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
            IServerTask task = null;
            DateTime lastUpdate = DateTime.Now;

            while (true) // loop until threadpool is disposed
            {
                lock (this._tasks) // finding a task needs to be atomic
                {
                    while (true) // wait for our turn in _workers queue and an available task
                    {
                        if (this._disposed)
                        {
                            return;
                        }
                        if (null != _workers.First && ReferenceEquals(Thread.CurrentThread, _workers.First.Value) &&
                            _tasks.Count > 0)
                        // we can only claim a task if its our turn (this worker thread is the first entry in _worker queue) and there is a task available
                        {
                            task = _tasks.First.Value;
                            _tasks.RemoveFirst();
                            _workers.RemoveFirst();
                            Monitor.PulseAll(_tasks);
                            // pulse because current (First) worker changed (so that next available sleeping worker will pick up its task)
                            break; // we found a task to process, break out from the above 'while (true)' loop
                        }
                        Monitor.Wait(_tasks); // go to sleep, either not our turn or no task to process
                    }
                }


                Thread.Sleep(1);


                bool Res = await Task.Run(() => UpdateTaskMain(task, lastUpdate));

               
                lock (this._tasks)
                {
                    if (Res)
                    {
                        _tasks.AddLast(task);
                        Monitor.PulseAll(_tasks);
                    }
                    this._workers.AddLast(Thread.CurrentThread);
                }
            }
        }
    }
}
