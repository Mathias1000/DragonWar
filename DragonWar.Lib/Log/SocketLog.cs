using System;
using DragonWar.Lib.Log;
using DragonWar.Lib.Module;

public sealed class SocketLog : FileLog
{
    protected override string AssertTypeName
    {
        get { return SocketLogType.Exception.ToString(); }
    }


    private SocketLog(string Directory)
        : base(Directory)
    {
    }










    private static SocketLog Instance { get { return (_Instance ?? (_Instance = new SocketLog(ServerMainBase.InternalInstance.StartDirectory + "Socket"))); } }
    private static SocketLog _Instance;


    public static void Write(SocketLogType Type, string Message, params object[] args)
    {
      
        Instance.Write(Type.ToString(), Message, args);
    }
    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, Message, args);
    }
}