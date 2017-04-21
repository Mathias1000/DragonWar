using System;
using System.Collections.Concurrent;
using System.IO;

namespace DragonWar.Utils.Logging
{
    public class FileLog
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





        private ConcurrentDictionary<string, StreamWriter> Writers;
        private object IOLocker;


        public FileLog(string Directory)
        {
            this.Directory = Directory.ToEscapedString();


            if (!System.IO.Directory.Exists(this.Directory))
                System.IO.Directory.CreateDirectory(this.Directory);


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

                    if (uint.TryParse(split[0], out uint ID) && ID >= SessionID)
                    {
                        SessionID = (ID + 1);
                    }
                }
            }

            SessionTime = DateTime.Now;
        }

        public void Write(string LogName,string LogType, string Message)
        {
            try
            {
                lock (IOLocker)
                {
                    if (!Writers.TryGetValue(LogName.ToLower(), out StreamWriter writer))
                    {
                        writer = new StreamWriter(String.Format("{0}{1}_{2}_{3}.txt", Directory, SessionID, LogName, SessionTime.ToString("MM_dd_yyyy"))) { AutoFlush = true };
                        Writers.TryAdd(LogName.ToLower(), writer);

                    }
                    writer.WriteLine(Message);
                }
            }
            catch (Exception)
            {
            }
        }

      
    }
}
