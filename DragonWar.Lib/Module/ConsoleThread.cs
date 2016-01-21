using System;
using System.Threading;
using System.Linq;


namespace DragonWar.Lib.Module
{
    public class ConsoleThread
    {
        private Thread CmdThread { get; set; }
     
        public ConsoleThread()
        {
            CmdThread = new Thread(StartRead);
            CmdThread.Start();
        }
        
        public void StartRead()
        {
            while(ServerMainBase.InternalInstance.IsRunning)
            {
                string Line = Console.ReadLine();
                string[] args = Line.Split(' ');
                if(args.Length >= 1)
                {
                    string cmd = args[0];
                    /*if(ConsoleCommandHandlerStore.InvokeConsoleCommand(cmd.ToLower(), args.Skip(1).ToArray()))
                    {
                      
                        EngineLog.WriteConsole(EngineConsoleLogType.Info, "Executing ConsoleCommand {0} Success!", cmd);
                    }
                    else
                    {
                        EngineLog.WriteConsole(EngineConsoleLogType.Error, "Can't find Command {0}",Line);
                    }*/
                }   
            }
        }
    }
}
