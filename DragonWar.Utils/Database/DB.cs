using System.Collections.Generic;
using DragonWar.Utils.Config.Section;
using System.Data.SqlClient;
using DragonWar.Utils.Database;
using DragonWar.Utils.Core;

public class DB
{
    private static DatabaseManager mManager { get; set; }

    public static bool Start(DatabaseSection DBSection)
    {
        mManager = new DatabaseManager(GenerateDatabaseServer(DBSection), GenerateDatabase(DBSection));

        if (mManager.TestConnection())
        {
            DatabaseLog.Write(DatabaseLogLevel.Startup, "Testing  Database Connection Settings Success!");


            ServerMainBase.InternalInstance.AddTask(mManager);

            return true;
        }
        else
        {
            DatabaseLog.Write(DatabaseLogLevel.DatabaseClientError, "Failed To Connect Please Check you Database Settings");

            return false;
        }
    }

    private static DatabaseServer GenerateDatabaseServer(DatabaseSection DBConf)
    {
        return new DatabaseServer(DBConf.SQLHost, DBConf.SQLUser, DBConf.SQLPassword);
    }

    private static Database GenerateDatabase(DatabaseSection DBConf)
    {
        return new Database(DBConf.SQLName, DBConf.MinPoolSize, DBConf.MaxPoolSize, DBConf.DatabaseClientLifeTime);
    }

    public static DatabaseClient GetDatabaseClient() => mManager.GetClient();

    public static void RunSQL(string sql, params SqlParameter[] parameter) => mManager.RunSQL(sql, parameter);

    public static SQLResult Select(string sql, params SqlParameter[] Parameters) => mManager.Select(sql, Parameters);

    public static void Dispose() => mManager?.Dispose();
    
}

