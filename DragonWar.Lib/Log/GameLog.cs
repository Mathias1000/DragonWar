using System;
using DragonWar.Lib.Log;
using DragonWar.Lib.Module;

public sealed class GameLog : FileLog
{
    protected override string AssertTypeName
    {
        get { return GameLogType.Exception.ToString(); }
    }


    private GameLog(string Directory)
        : base(Directory)
    {
    }







    private static GameLog Instance { get { return (_Instance ?? (_Instance = new GameLog(ServerMainBase.InternalInstance.StartDirectory + "Game"))); } }
    private static GameLog _Instance;


    public static void Write(GameLogType Type, string Message, params object[] args)
    {
        Instance.Write(Type.ToString(), Message, args);
    }
    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, Message, args);
    }
}