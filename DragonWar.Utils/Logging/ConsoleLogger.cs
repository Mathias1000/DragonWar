using System;

namespace DragonWar.Utils.Logging
{
    public class ConsoleLogger
    {
        private object IOLocker;


       
        public ConsoleLogger()
        {

            IOLocker = new object();
        }
        public void WriteLine(dynamic LogTypeName, dynamic LogType, string Message)
        {
            lock (IOLocker)
            {
                if (ConsoleColors.GetColor(LogTypeName, (byte)LogType, out ConsoleColor pColor))
                {

                    Console.ForegroundColor = pColor;
                    Console.WriteLine(Message);
                    Console.ResetColor();

                }
            }
        }

        public void ClearCurrentConsoleLine()
        {
            /*
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);*/
        }
        //Use as Header for bar :)
        public void WriteConsoleLine(ConsoleColor pColor, string Text, params Object[] args)
        {
            lock (IOLocker)
            {
                Console.ForegroundColor = pColor;
                String ma = string.Format("{0} \n", Text, args);
                Console.WriteLine( Text, args);
                Console.ResetColor();
                Console.Out.Flush();
            }
        }
    }
}
