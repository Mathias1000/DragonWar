using System;

namespace DragonWar.Utils.ServerTask
{
    public interface IServerTask : IDisposable
    {
        bool Update(GameTime Now);
        event TaskEvenHandler OnLeave;
        event TaskEvenHandler OnEnter;
        ServerTaskTimes Intervall { get; set; }
        GameTime LastUpdate { get; set; }
        void InvokeOnEnter(GameTime Now);
        void InvokeOnLeave(GameTime Now);
    }
}
