﻿using System;
using DragonWar.Utils.Config.Section.Log;

namespace DragonWar.Utils.Config
{
    [ServerModule(ServerType.Match, InitializationStage.PreData)]
    [ServerModule(ServerType.Service, InitializationStage.PreData)]
    public class LogConfiguration : Configuration<LogConfiguration>
    {
        public static LogConfiguration Instance { get; set; }

        public CommandLogSection Log_Command  = new CommandLogSection();

        public DatabaseLogSection Log_DB  = new DatabaseLogSection();

        public EngineLogSection Log_Engine  = new EngineLogSection();

        public GameLogSection Log_Game  = new GameLogSection();

        public SocketLogSection Log_Socket  = new SocketLogSection();


        public void SetupConsole()
        {
            CommandLog.SetupLevels(Log_Command.ConsoleLogLevel, Log_Command.FileLogLevel);
            DatabaseLog.SetupLevels(Log_DB.ConsoleLogLevel, Log_DB.FileLogLevel);
            EngineLog.SetupLevels(Log_Engine.ConsoleLogLevel, Log_Engine.FileLogLevel);
            GameLog.SetupLevels(Log_Game.ConsoleLogLevel, Log_Game.FileLogLevel);
            SocketLog.SetupLevels(Log_Socket.ConsoleLogLevel, Log_Socket.FileLogLevel);
        }
       
        public static bool Write(out LogConfiguration pConfig)
        {
            pConfig = null;
            try
            {
                pConfig = new LogConfiguration();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [InitializerMethod]
        public static bool Initialize()
        {
            try
            {
                Instance = ReadXml();

                if (Instance != null)
                {
                    Instance.SetupConsole();
                    EngineLog.Write(EngineLogLevel.Startup, "Successfully read Log config.");
                    return true;
                }
                else
                {

                    LogConfiguration pConfig;
                    if (Write(out pConfig))
                    {
                        pConfig.WriteXml();
                        EngineLog.Write(EngineLogLevel.Startup, "Successfully created Log config.");
                        return false;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Failed to Load config {0}", ex);
                return false;
            }
        }
    }
}
