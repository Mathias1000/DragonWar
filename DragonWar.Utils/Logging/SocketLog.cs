using DragonWar.Utils.Logging;
using System;

public sealed class SocketLog : LoggerBase
{

    protected override string LogTypeName => "SocketLog";

    private SocketLog(string Directory)
  : base(Directory)
    {
    }

    private static SocketLog Instance { get { return (_Instance ?? (_Instance = new SocketLog(@"Socket"))); } }
    private static SocketLog _Instance;

    public static void SetupLevels(byte mConsolenLevel, byte mFileLogLevel)
    {
        Instance.SetConsolenLevel(mConsolenLevel);
        Instance.SetFileLogLevel(mFileLogLevel);
    }

    public static void Write(SocketLogLevel Type, string Message, params object[] args)
    {
        Instance.Write(Instance.ToString(), Type, Message, args);
    }

    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, SocketLogLevel.Exception, Message, args);
    }
}
