using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace DragonWar.Utils.Database
{
    public sealed class QueryPool : IDisposable
    {
        private readonly LinkedList<Thread> _workers; // queue of worker threads ready to process actions
        private readonly LinkedList<PoolQuery> CachedQuerys = new LinkedList<PoolQuery>();
        private bool _disallowAdd; // set to true when disposing queue but there are still tasks pending
        private bool _disposed; // set to true when disposing queue and no more tasks are pending
        private DatabaseManager pManager;
     
        public QueryPool(DatabaseManager _Manager,int size)
        {
            this._workers = new LinkedList<Thread>();
            this.pManager = _Manager;
            for (var i = 0; i < size; ++i)
            {
                var worker = new Thread(this.Worker) { Name = string.Concat("SqlWorker ", i) };
                worker.Start();
                this._workers.AddLast(worker);
            }
        }

        public void Dispose()
        {
            var waitForThreads = false;
            lock (this.CachedQuerys)
            {
                if (!this._disposed)
                {
                    GC.SuppressFinalize(this);

                    this._disallowAdd = true; // wait for all tasks to finish processing while not allowing any more new tasks
                    while (this.CachedQuerys.Count > 0)
                    {
                        Monitor.Wait(this.CachedQuerys);
                    }

                    this._disposed = true;
                    Monitor.PulseAll(this.CachedQuerys); // wake all workers (none of them will be active at this point; disposed flag will cause then to finish so that we can join them)
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

        public void AddQuery(SqlCommand cmd)
        {
            lock (this.CachedQuerys)
            {
                if (this._disallowAdd) { throw new InvalidOperationException("This SqlPool instance is in the process of being disposed, can't add anymore"); }
                if (this._disposed) { throw new ObjectDisposedException("This SqlPool instance has already been disposed"); } 


             
                CachedQuerys.AddLast(new PoolQuery(cmd));
                Monitor.PulseAll(this.CachedQuerys); // pulse because tasks count changed
            }
        }

        private void ExecuteQuery(PoolQuery mPoolQuery)
        {
            using (DatabaseClient pClient = pManager.GetClient())
            {
                try
                {
                    pClient.ExecuteNonQuery(mPoolQuery.pCmd);

                }
                catch (SqlException ex)
                {
                    DatabaseLog.Write(DatabaseLogLevel.Error, "Failed to Execute Query {0} {1}", mPoolQuery.pCmd.CommandText, ex.Message);
                }

            }
        }

        private void Worker()
        {
            PoolQuery mQuery = null;
            while (true) // loop until threadpool is disposed
            {
                lock (this.CachedQuerys) // finding a task needs to be atomic
                {
                    while (true) // wait for our turn in _workers queue and an available task
                    {
                        if (this._disposed)
                        {
                            return;
                        }
                        if (null != this._workers.First && object.ReferenceEquals(Thread.CurrentThread, this._workers.First.Value) && this.CachedQuerys.Count > 0) // we can only claim a task if its our turn (this worker thread is the first entry in _worker queue) and there is a task available
                        {
                            mQuery = this.CachedQuerys.First.Value;
                            this.CachedQuerys.RemoveFirst();
                            this._workers.RemoveFirst();
                            Monitor.PulseAll(this.CachedQuerys); // pulse because current (First) worker changed (so that next available sleeping worker will pick up its task)
                            break; // we found a task to process, break out from the above 'while (true)' loop
                        }
                        Monitor.Wait(this.CachedQuerys); // go to sleep, either not our turn or no task to process
                    }
                }

                ExecuteQuery(mQuery);

                lock (this.CachedQuerys)
                {
                    this._workers.AddLast(Thread.CurrentThread);
                }
                mQuery = null;
            }
        }
    }
}
