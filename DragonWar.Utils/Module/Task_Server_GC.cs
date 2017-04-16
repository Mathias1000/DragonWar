using System;

using DragonWar.Utils.ServerTask;

namespace DragonWar.Utils.Module.Server
{
   [ServerTaskClass(ServerTaskTimes.SERVER_GC_INTERVAL)]
    public class Task_Server_GC : mServerTask
    {
        public override void Dispose()
        {
        
        }
     
        public override bool Update(GameTime Now)
        {
            GC.Collect();
            return true;
        }
    }
}
