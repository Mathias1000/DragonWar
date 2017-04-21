using DragonWar.Utils.Config;
using DragonWar.Utils.Config.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.LobbyClient.Config
{
    public class LobbyClientConfiguration : Configuration<LobbyClientConfiguration>
    {

        public static LobbyClientConfiguration Instance { get; set; }


        public ConnectSection ConnectInfo { get; set; } = new ConnectSection();

        public bool Debug { get; set; } = true;

        public static bool Initialize()
        {
            try
            {
                Instance = ReadXml();

                if (Instance != null)
                {
                    EngineLog.Write(EngineLogLevel.Startup, "Successfully read Servcice config.");
                    return true;
                }
                else
                {

                    if (Write(out LobbyClientConfiguration pConfig))
                    {
                        pConfig.WriteXml();
                        EngineLog.Write(EngineLogLevel.Startup, "Successfully created Service config.");
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

        public static bool Write(out LobbyClientConfiguration pConfig)
        {
            pConfig = null;
            try
            {
                pConfig = new LobbyClientConfiguration();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
