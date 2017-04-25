using DragonWar.Utils.Config;
using DragonWar.Utils.Config.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.MatchServer.Config
{
    public class MatchServerConfiguration : Configuration<MatchServerConfiguration>
    {
        public ServerSection MatchServerInfo { get; set; } = new ServerSection();

        public ConnectSection ConnectInfo { get; set; } = new ConnectSection();


        public int MinPort { get; set; } = 5000;

        public int MaxPort { get; set; } = 5002;

        public string GameIP { get; set; } = "127.0.0.1";

        public int WorkThreadCount { get; set; } = 4;

        public static MatchServerConfiguration Instance { get; set; }

        public static bool Initialize()
        {
            try
            {
                Instance = ReadXml();

                if (Instance != null)
                {
                    EngineLog.Write(EngineLogLevel.Startup, "Successfully read MatchServer config.");
                    return true;
                }
                else
                {

                    if (Write(out MatchServerConfiguration pConfig))
                    {
                        pConfig.WriteXml();
                        EngineLog.Write(EngineLogLevel.Startup, "Successfully created MatchServer config.");
                        return false;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Failed to Load config {0}", ex);
                return false;
            }
        }


        public static bool Write(out MatchServerConfiguration pConfig)
        {
            pConfig = null;
            try
            {
                pConfig = new MatchServerConfiguration();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
