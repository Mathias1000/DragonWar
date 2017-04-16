namespace DragonWar.Utils.Config.Section
{
    public class ServerSection
    {

        public virtual string ListenerIP { get; set; } = "0.0.0.0";

        public virtual int Port { get; set; } = 8800;

        public virtual int NetworThreads { get; set; } = 5;

        public virtual ushort MaxConnection { get; set; } = 50;

        public virtual string ServerPassword { get; set; } = "Dubistdoof";
    }
}
