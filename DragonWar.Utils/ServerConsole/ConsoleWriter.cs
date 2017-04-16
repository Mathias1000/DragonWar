using System;

namespace DragonWar.Utils.ServerConsole
{
    public static class ConsoleWriter
    {
        public static void WriteLine(string format, params object[] args) { System.Console.WriteLine(format, args); }
        public static void Write(string format, params object[] args) { System.Console.Write(format, args); }
        public static int Y { get { return Console.CursorTop; } set { System.Console.CursorTop = value; } }
        public static int X { get { return Console.CursorLeft; } set { System.Console.CursorLeft = value; } }
        public static int WindowWidth() { return Console.WindowWidth; }
        public static ConsoleColor ForegroundColor { get { return Console.ForegroundColor; } set { System.Console.ForegroundColor = value; } }
        public static void PrintAt(int x, int y, string format, params object[] args)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
            Console.WriteLine(format, args);
        }
        public static void PrintAt(int x, int y, string text)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
            Console.WriteLine(text);
        }

    }
}
