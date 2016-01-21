using System;
using System.Linq;
using DragonWar.Lib.Util;
using System.Reflection;


namespace DragonWar.Lib.Module
{
    public class ServerMainBase
    {
        public static ServerMainBase InternalInstance { get; private set; }

        public string StartDirectory { get; private set; }
        public string StartExecutable { get; private set; }
        public bool IsRunning { get; private set; }


        public GameTime CurrentTime { get; internal set; }
        public TimeSpan TotalUpTime { get; internal set; }

        private ConsoleThread CmdThread { get; set; }

        public ServerMainBase()
        {
            if (InternalInstance != null)
                throw new InvalidOperationException("Can only load one instance of this class at once.");
            InternalInstance = this;



            StartDirectory = AppDomain.CurrentDomain.BaseDirectory.ToEscapedString();
            StartExecutable = (Assembly.GetEntryAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\"));


            CurrentTime = (GameTime)DateTime.Now;
            

        }

        public virtual bool LoadServer()
        {
            if (LoadServerModules())
            {
                IsRunning = true;
                EngineWorker.Initialize();
                CmdThread = new ConsoleThread();
                return true;
            }
            return false;
        }

        public bool LoadServerModules()
        {
            if (Reflector.GetInitializerMethods().Any(method => !method.Invoke()))
            {
                EngineLog.Write(EngineLogType.Exception ,"Server could not be started. Errors occured.");
                return false;
            }

            return true;
        }
        public void Shutdown()
        {
         
            foreach (var m in Reflector.GetCleanupMethods())
            {
                m.Invoke();
            }
            IsRunning = false;
        }
    }
}
