using DragonWar.Utils.Config.Section;

namespace DragonWar.Service.Config
{
    public class ServiceDatabaseSection : DatabaseSection
    {
        public override string SQLName => "DragonWar_Service";
    }
}
