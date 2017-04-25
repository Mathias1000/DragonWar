using System;

namespace DragonWar.Utils.ServerTask
{
    public delegate void TaskEvenHandler(GameTime Now);

#pragma warning disable IDE1006 // Benennungsstile
    public abstract class mServerTask : IServerTask
#pragma warning restore IDE1006 // Benennungsstile
    {
        public ServerTaskTimes Intervall { get; set; }
        public GameTime LastUpdate { get; set; }

       
        public event TaskEvenHandler OnLeave;
        public event TaskEvenHandler OnEnter;

        public mServerTask()
        {
            LastUpdate = (GameTime)DateTime.Now;
        }

        public mServerTask(ServerTaskTimes _intervall)
        {
            Intervall = _intervall;
            LastUpdate = (GameTime)DateTime.Now;
        }

        public mServerTask(int _intervall)
        {
            Intervall = (ServerTaskTimes)_intervall;
            LastUpdate = (GameTime)DateTime.Now;
        }

        public abstract bool Update(GameTime Now);

        public void InvokeOnEnter(GameTime Now) => OnEnter?.Invoke(Now); 
   
        public void InvokeOnLeave(GameTime Now) => OnLeave?.Invoke(Now); 


       public abstract void Dispose();

       
    }
}
