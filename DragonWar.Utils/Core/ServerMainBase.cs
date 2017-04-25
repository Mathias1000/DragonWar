using System;
using System.Linq;
using System.Reflection;
using DragonWar.Utils.ServerTask;


using DragonWar.Utils.ServerConsole;


namespace DragonWar.Utils.Core
{
    public class ServerMainBase
    {
        public static ServerMainBase InternalInstance { get; private set; }

        public ServerType ServerType { get; private set; }
        public string StartDirectory { get; private set; }
        public string StartExecutable { get; private set; }
        public bool ServerIsReady = false;

        public GameTime CurrentTime { get; internal set; }
        public TimeSpan TotalUpTime { get; internal set; }

        private ConsoleThread CmdThread { get; set; }
        public TaskPool ThreadPool { get; private set; }

        public ServerMainBase(ServerType pType)
        {
            if (InternalInstance != null)
                throw new InvalidOperationException("Can only load one instance of this class at once.");
            InternalInstance = this;
            LoadExsternAssemblys();//Load in Assmebly cache and fix gloabals load bug-...
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            StartDirectory = AppDomain.CurrentDomain.BaseDirectory.ToEscapedString();
            StartExecutable = (Assembly.GetEntryAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\"));



            CurrentTime = (GameTime)DateTime.Now;
            ServerType = pType;

        }
        private void LoadExsternAssemblys()
        {
            Assembly.Load(@"DragonWar.Utils");
            Assembly.Load(@"DragonWar.Game");
            Assembly.Load(@"DragonWar.Networking");
        }
        public void WriteConsoleLogo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("****************************************************************");
            Console.WriteLine("_____                          __          __                   ");
            Console.WriteLine(" |  __ \\                         \\ \\        / /              ");
            Console.WriteLine(" | |  | |_ __ __ _  __ _  ___  _ _\\ \\  /\\  / /_ _ _ __       ");
            Console.WriteLine(" | |  | | '__/ _` |/ _` |/ _ \\| '_ \\ \\/  \\/ / _` | '__      ");
            Console.WriteLine(" | |__| | | | (_| | (_| | (_) | | | \\  /\\  / (_| | |          ");
            Console.WriteLine(" |_____/|_|  \\__,_|\\__, |\\___/|_| |_|\\/  \\/ \\__,_|_|      ");
            Console.WriteLine("                    __/ |                                       ");
            Console.WriteLine("                   |___/   Copyright 2017 by Mathias1000        ");
            Console.WriteLine("****************************************************************");
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            EngineLog.Write(EngineLogLevel.Exception, e.ExceptionObject.ToString());

        }

        public virtual bool LoadBaseServerModule()
        {

            if (LoadServerModules())
            {
                CmdThread = new ConsoleThread();


                return true;
            }
            return false;
        }

        public void LoadThreadPool(int ThreadCount)
        {

            ThreadPool = new TaskPool(ThreadCount);

            //Adding Tasks

            if (!AddRunTimeTasks())
            {
                Shutdown();
            }

        }

        private bool AddRunTimeTasks()
        {

            var TaskList = Reflector.GiveServerTasks();

            foreach (var mTask in TaskList)
            {
                try
                {
                    var mU = (IServerTask)Activator.CreateInstance(mTask.Second);
                    mU.Intervall = mTask.First;
                    AddTask(mU);
                }
                catch (Exception ex)
                {
                    EngineLog.Write(EngineLogLevel.Exception, "Failed to Add Task {0} Error : {1}", mTask.Second, ex.Message);
                    return false;
                }
            }
            EngineLog.Write(EngineLogLevel.Startup, "Adding {0} ServerTasks", TaskList.Count());
            return true;


        }
        public bool LoadGameServerModules() //Load All Game Class After Authenticate
        {
            try
            {

                foreach (var m in Reflector.GetStartUPCleans(ServerType))
                {
                    m.Invoke();
                }

                if (Reflector.GetInitializerGameMethods(this.ServerType).Any(method => !method.Invoke()))
                {
                    GameLog.Write(GameLogLevel.Exception, "GameServer could not be started. Errors occured.");
                    return false;
                }



            }
            catch (Exception ex)
            {

                GameLog.Write(GameLogLevel.Exception, "GameServer could not be started. Errors occured {0}.", ex.ToString());
                return false;
            }

            return true;
        }
        public bool LoadServerModules()
        {
            try
            {


                if (Reflector.GetInitializerServerMethods(this.ServerType).Any(method => !method.Invoke()))
                {
                    EngineLog.Write(EngineLogLevel.Exception, "Server could not be started. Errors occured.");
                    return false;
                }


            }
            catch (Exception ex)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Server could not be started. Errors occured {0}.", ex.ToString());
                return false;
            }

            return true;
        }

        public virtual void Shutdown()
        {
            CmdThread.StopRead();

            foreach (var m in Reflector.GetCleanupServerMethods(ServerType))
            {
                m.Invoke();
            }


        }

        public void AddTask(IServerTask mTask)
        {
            ThreadPool.QueueTask(mTask);
        }


    }
}
