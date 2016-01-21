using System;
using System.Threading;
using DragonWar.Lib.Module;

namespace DragonWar.Lib
{
    internal static class EngineWorker
    {
        internal static void Initialize()
        {
            new Thread(Work).Start();
        }

        private static void Work()
        {
            DateTime lastUpdate = DateTime.Now,
                     lastGC = DateTime.Now;

            while (ServerMainBase.InternalInstance.IsRunning)
            {
                try
                {
                    var now = DateTime.Now;
                    var elapsed = (now - lastUpdate);

                    lastUpdate = now;
                    ServerMainBase.InternalInstance.TotalUpTime += elapsed;

                    var gameTime = new GameTime(now, elapsed, ServerMainBase.InternalInstance.TotalUpTime);
                    ServerMainBase.InternalInstance.CurrentTime = gameTime;

                    if (now.Subtract(lastGC).TotalSeconds >= 30)
                    {
                        GC.Collect();

                        lastGC = now;
                    }
                }
                catch (Exception ex)
                {
                    if (ServerMainBase.InternalInstance.IsRunning)
                    {
                        EngineLog.Write(ex, "Error performing worker functions:");
                    }
                }
                finally
                {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
