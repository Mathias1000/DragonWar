using DragonWar.Utils.Config;
using DragonWar.Utils.Config.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.Config
{
    public class ServiceConfiguration : Configuration<ServiceConfiguration>
    {
        public ServerSection Service { get; set; } = new ServerSection();

        public ServerSection GameServer { get; set; } = new ServerSection();


        public ServiceDatabaseSection DatabaseInfo { get; set; } = new ServiceDatabaseSection();

        public static ServiceConfiguration Instance { get; set; }


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

                    if (Write(out ServiceConfiguration pConfig))
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


        public static bool Write(out ServiceConfiguration pConfig)
        {
            pConfig = null;
            try
            {
                pConfig = new ServiceConfiguration();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
