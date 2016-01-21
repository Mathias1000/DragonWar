using DragonWar.Lib.Log;
using DragonWar.Lib.Module;

public class CommandLog : FileLog
{
    protected override string AssertTypeName
    {
        get { return GameLogType.Internal.ToString(); }
    }
    private CommandLog(string Directory)
        : base(Directory)
    {
    }
    
    private static CommandLog Instance { get { return (_Instance ?? (_Instance = new CommandLog(ServerMainBase.InternalInstance.StartDirectory + "Command"))); } }
    private static CommandLog _Instance;


}

