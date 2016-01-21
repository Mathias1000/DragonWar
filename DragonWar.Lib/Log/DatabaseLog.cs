using System;
using DragonWar.Lib.Log;
using DragonWar.Lib.Util;
using DragonWar.Lib.Module;

public sealed class DatabaseLog : FileLog
{
    protected override string AssertTypeName
    {
        get { return GameLogType.Exception.ToString(); }
    }


    private DatabaseLog(string Directory)
        : base(Directory)
    {
    }







    private static DatabaseLog Instance { get { return (_Instance ?? (_Instance = new DatabaseLog(ServerMainBase.InternalInstance.StartDirectory + "Game"))); } }
    private static DatabaseLog _Instance;


    public static void Write(DatabaseLogType Type, string Message, params object[] args)
    {
        Instance.Write(Type.ToString(), Message, args);
    }
    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, Message, args);
    }
}