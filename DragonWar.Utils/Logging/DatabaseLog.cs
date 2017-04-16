using DragonWar.Utils.Logging;
using System;

public sealed class DatabaseLog : LoggerBase
{

    protected override string LogTypeName => "DatabaseLog";

    private DatabaseLog(string Directory)
  : base(Directory)
    {
    }



    private static DatabaseLog Instance { get { return (_Instance ?? (_Instance = new DatabaseLog("Database"))); } }
    private static DatabaseLog _Instance;

    public static void SetupLevels(byte mConsolenLevel, byte mFileLogLevel)
    {
        Instance.SetConsolenLevel(mConsolenLevel);
        Instance.SetFileLogLevel(mFileLogLevel);
    }
    public static void WriteConsole(string Message, params object[] args)
    {
        Instance.WriteConsole(ConsoleColor.Green, Message, args);
    }
    public static void Write(DatabaseLogLevel Type, string Message, params object[] args)
    {
        Instance.Write(Instance.ToString(), Type, Message, args);
    }

    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, DatabaseLogLevel.DatabaseClientError, Message, args);
    }
}

