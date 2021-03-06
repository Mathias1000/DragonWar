﻿using DragonWar.Utils.Logging;
using System;

public sealed class GameLog : LoggerBase
{
    protected override string LogTypeName => "GameLog";

    private GameLog(string Directory)
      : base(Directory)
    {
    }



    private static GameLog Instance { get { return (_Instance ?? (_Instance = new GameLog(@"Game"))); } }
    private static GameLog _Instance;


    public static void SetupLevels(byte mConsolenLevel, byte mFileLogLevel)
    {
        Instance.SetConsolenLevel(mConsolenLevel);
        Instance.SetFileLogLevel(mFileLogLevel);
    }

    public static void Write(GameLogLevel Type, string Message, params object[] args)
    {
        Instance.Write(Instance.ToString(), Type, Message, args);
    }

    public static void Write(Exception Exception, string Message, params object[] args)
    {
        Instance.WriteException(Exception, GameLogLevel.Exception, Message, args);
    }

}
