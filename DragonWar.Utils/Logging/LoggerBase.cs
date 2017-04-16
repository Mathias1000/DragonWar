using System;

namespace DragonWar.Utils.Logging
{
    public  class LoggerBase
    {

        private ConsoleLogger ConsoleLogger { get; set; }

        private FileLog FileLogger { get; set; }


        protected internal byte mFileLogLevel = byte.MaxValue;

        protected internal byte mConsoleLogLevel = byte.MaxValue;


        protected virtual string LogTypeName { get; }


        public LoggerBase(string FileDirectory)
        {
            ConsoleLogger = new ConsoleLogger();
            FileLogger = new FileLog(FileDirectory);
          
        }


        public void SetConsolenLevel(byte LogLevel)
        {
            mConsoleLogLevel = LogLevel;
        }

        public void SetFileLogLevel(byte LogLevel)
        {
            mFileLogLevel = LogLevel;
        }
        public void WriteConsole(ConsoleColor pColor,string Message, params object[] args)
        {
            ConsoleLogger.WriteConsoleLine(pColor, Message, args);
        }

        public void Write(string LogTypeName, dynamic LogType, string Message, params object[] args)
        {
            string msg = (String.Format("[{0}][{1}][{2}] {3}", DateTime.Now,LogTypeName, LogType, String.Format(Message, args)));
            if ((byte)LogType <= mFileLogLevel)
            {
                FileLogger.Write(LogTypeName.ToString(), LogType.ToString(), msg);
            }

            if ((byte)LogType <= mConsoleLogLevel)
            {
                ConsoleLogger.ClearCurrentConsoleLine();
                ConsoleLogger.WriteLine(LogTypeName, LogType, msg);
            }
        }


        public void WriteException(Exception Exception, dynamic LogType, string Commend, params object[] args)
        {
            Write(LogTypeName,LogType, String.Format("{0}{1}{1}{1}{2}{1}{1}{3}{1}{1}{1}", Commend, Environment.NewLine, Exception.Message, Exception.StackTrace),  args);
        }
    }
}
