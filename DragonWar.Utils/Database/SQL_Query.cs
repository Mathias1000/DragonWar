using DragonWar.Utils.ServerTask;
using System.Data.SqlClient;
using System;

namespace DragonWar.Utils.Database
{
    public sealed class SQL_Query : mServerTask
    {
        public SqlCommand pCmd { get; private set; }

        public SQL_Query(SqlCommand cmd)
        {
            pCmd = cmd;
            OnLeave += SQL_Query_OnLeave;
        }

        private void SQL_Query_OnLeave(GameTime Now)
        {
            using (DatabaseClient pClient = DB.GetDatabaseClient())
            {
                try
                {
                    pClient.ExecuteNonQuery(pCmd);

                }
                catch (SqlException ex)
                {
                    DatabaseLog.Write(DatabaseLogLevel.Error, "Failed to Execute Query {0} {1}", pCmd.CommandText, ex.Message);
                }

            }
        }

        public override bool Update(GameTime Now)
        {
            //Note needet...
            return false;
        }

        public override void Dispose()
        {
          
        }
    }
}
