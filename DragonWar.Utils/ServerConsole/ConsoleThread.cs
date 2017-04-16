using System;
using System.Threading;
using DragonWar.Utils.Core;

namespace DragonWar.Utils.ServerConsole
{
    public class ConsoleThread
    {
        private Thread CmdThread { get; set; }
     
        public ConsoleThread()
        {
            CmdThread = new Thread(StartRead) { Name = string.Concat("CmdThread") };
            System.Threading.Thread.Sleep(200);
            CmdThread.Start();
            
        }

        public void StopRead()
        {
            CmdThread.Abort();

        }
        public void StartRead()
        {
            string Input = string.Empty;
           
            ConsoleKeyInfo cki;
            do
            {
   
                // Your code could perform some useful task in the following loop. However, 
                // for the sake of this example we'll merely pause for a quarter second.

                while (Console.KeyAvailable == false)
                    Thread.Sleep(250); // Loop until input is entered.


                cki = Console.ReadKey(true);

                Console.Write(cki.KeyChar);

                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    string[] args = Input.Split(' ');
                    if (args.Length >= 1)
                    {
                        string cmd = args[0];
                        if (!ConsoleCommandHandlerStore.InvokeConsoleCommand(cmd.ToUpper(), args))
                        {
                            CommandLog.Write(CommandLogLevel.Console, "Can't find Command {0}", Input);
                        }
                    }
                    Input = string.Empty;
                    continue;
                }
                else if (cki.Key == ConsoleKey.Backspace)
                {
                    if (Input.Length > 0)
                    {
                        Input = Input.Substring(0, Input.Length - 1);
                      
                        int x = Console.CursorLeft;
                        int y = Console.CursorTop;
            
                        Console.Write("\r{0} ", Input);
                        // Restore previous position
                        Console.SetCursorPosition(x, y);

                    }
                    continue;
                }
                Input += cki.KeyChar;

            } while (ServerMainBase.InternalInstance.ServerIsReady);

        }
    }
}
