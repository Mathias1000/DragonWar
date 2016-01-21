using System;
using System.Collections.Concurrent;

namespace DragonWar.Lib.Log
{
    public class ConsoleLog
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, ConsoleColor>> ConsoleColors { get; set; }
        public ConsoleLog()
        {
            SetupColors();
        }

        public void Write(string LogType, string LogName, string Message)
        {
            ConcurrentDictionary<string, ConsoleColor> GameColors;
            if (ConsoleColors.TryGetValue(LogType, out GameColors))
            {
                ConsoleColor Color;
                if (GameColors.TryGetValue(LogName, out Color))
                {
                    Console.ForegroundColor = Color;
                    Console.WriteLine(Message);
                }
            }

            Console.ResetColor();
        }

        public void Write(string LogType,string Message)
        {
           ConcurrentDictionary<string, ConsoleColor> Colors;
            if(ConsoleColors.TryGetValue("Console",out Colors))
            {
                ConsoleColor Color;
                if (Colors.TryGetValue(LogType, out Color))
                {
                    Console.ForegroundColor = Color;
                    Console.WriteLine("["+LogType+"] "+Message);
                }
            }
            Console.ResetColor();
        }
     
        private void SetupColors()
        {
            ConsoleColors = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConsoleColor>>();

           var GameColors = new ConcurrentDictionary<string, ConsoleColor>();

            var SocketColors = new ConcurrentDictionary<string, ConsoleColor>();

            var ConsoleLogColors = new ConcurrentDictionary<string, ConsoleColor>();

            var DatabaseLogColors = new ConcurrentDictionary<string, ConsoleColor>();

            //GameLog ETC
            GameColors.TryAdd("Debug", ConsoleColor.Magenta);
            GameColors.TryAdd("Internal", ConsoleColor.Cyan);
            GameColors.TryAdd("Warning", ConsoleColor.Yellow);
            GameColors.TryAdd("Startup", ConsoleColor.Green);
            GameColors.TryAdd("Exception", ConsoleColor.Red);

            //SocketLog
            SocketColors.TryAdd("Debug", ConsoleColor.Blue);
            SocketColors.TryAdd("Internal", ConsoleColor.Blue);
            SocketColors.TryAdd("Warning", ConsoleColor.Blue);
            SocketColors.TryAdd("Startup", ConsoleColor.Blue);
            SocketColors.TryAdd("Exception", ConsoleColor.Blue);

            //Only Console No Logging into file
            SocketColors.TryAdd("Debug", ConsoleColor.DarkGray);
            SocketColors.TryAdd("Warning", ConsoleColor.DarkYellow);
            SocketColors.TryAdd("Info", ConsoleColor.Green);
            SocketColors.TryAdd("Error", ConsoleColor.Cyan);

            //Database Log Colors
            DatabaseLogColors.TryAdd("Debug", ConsoleColor.DarkYellow);
            DatabaseLogColors.TryAdd("Startup", ConsoleColor.Green);
            DatabaseLogColors.TryAdd("QueryError", ConsoleColor.DarkRed);

            ConsoleColors.TryAdd("GameLog", GameColors);
            ConsoleColors.TryAdd("EngineLog", GameColors);
            ConsoleColors.TryAdd("SocketLog", SocketColors);
            ConsoleColors.TryAdd("Console", SocketColors);
        }
    }
}
