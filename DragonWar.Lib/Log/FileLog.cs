using System;
using System.IO;
using System.Collections.Concurrent;
using DragonWar.Lib.Module;

namespace DragonWar.Lib.Log
{
    public abstract class FileLog : iLogWriter
    {
        /// <summary>
        /// Returns the path where the logs will be saved.
        /// </summary>
        public string Directory { get; private set; }


        /// <summary>
        /// Returns the ID of the current log session.
        /// </summary>
        public uint SessionID { get; private set; }


        /// <summary>
        /// Returns the time when this log session was started.
        /// </summary>
        public DateTime SessionTime { get; private set; }



        private ConsoleLog ConsoleLogger { get; set; }



        private ConcurrentDictionary<string, StreamWriter> Writers;
        private object IOLocker;

        protected abstract string AssertTypeName { get; }



        protected FileLog(string Directory)
        {
            this.Directory = Directory.ToEscapedString();


            if (!System.IO.Directory.Exists(this.Directory))
                System.IO.Directory.CreateDirectory(this.Directory);



            ConsoleLogger = new ConsoleLog();
            LoadSession();
            Writers = new ConcurrentDictionary<string, StreamWriter>();
            IOLocker = new object();
        }
        private void LoadSession()
        {
            foreach (var file in System.IO.Directory.GetFiles(Directory, "*.txt", SearchOption.TopDirectoryOnly))
            {
                var withoutEx = Path.GetFileNameWithoutExtension(file);

                if (withoutEx.Contains("_"))
                {
                    var split = withoutEx.Split('_');

                    uint ID;
                    if (uint.TryParse(split[0], out ID)
                    && ID >= SessionID)
                    {
                        SessionID = (ID + 1);
                    }
                }
            }

            SessionTime = DateTime.Now;
        }




        public void WriteConsole(string LogName,string Message,params object[] args)
        {
            ConsoleLogger.Write(LogName, string.Format(Message,args));
        }


        public void Write(string LogName, string Message, params object[] args)
        {
            try
            {
                lock (IOLocker)
                {
                    StreamWriter writer;
                    if (!Writers.TryGetValue(LogName.ToLower(), out writer))
                    {
                       
                        writer = new StreamWriter(String.Format("{0}{1}_{2}_{3}.txt", Directory, SessionID, LogName, SessionTime.ToString("MM_dd_yyyy"))) { AutoFlush = true };
                        Writers.TryAdd(LogName.ToLower(), writer);
                    }
             
                   ConsoleLogger.Write(this.GetType().ToString(),LogName, String.Format("[{0}][{1}] {2}", LogName, String.Format(Message, args)));
                    writer.WriteLine(String.Format("[{0}][{1}] {2}", ServerMainBase.InternalInstance.CurrentTime, LogName, String.Format(Message, args)));
                }
            }
            catch (Exception)
            {
            }
        }
        public void WriteException(Exception Exception, string Commend, params object[] args)
        {

            Write(AssertTypeName, String.Format("{0}{1}{1}{1}{2}{1}{1}{3}{1}{1}{1}", Commend, Environment.NewLine, Exception.Message, Exception.StackTrace), args);
        }
    }
}