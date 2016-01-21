using System;
using DragonWar.Lib.Log;
using DragonWar.Lib.Module;

public sealed class EngineLog : FileLog
{
    protected override string AssertTypeName
    {
        get { return EngineLogType.Exception.ToString(); }
    }


    private EngineLog(string Directory)
        : base(Directory)
    {
    }










    private static EngineLog Instance { get { return (_Instance ?? (_Instance = new EngineLog(ServerMainBase.InternalInstance.StartDirectory + "Engine"))); } }
    private static EngineLog _Instance;

    public static void WriteConsole(EngineConsoleLogType Type, string Message, params object[] args)
    {
        Instance.WriteConsole(Type.ToString(), Message,args);
    }

    public static void Write(EngineLogType Type, string Message, params object[] args)
    {
        Instance.Write(Type.ToString(), Message, args);
    }
    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, Message, args);
    }
}