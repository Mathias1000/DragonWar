using DragonWar.Utils.ServerTask;
using System.Data.SqlClient;
using System;

namespace DragonWar.Utils.Database
{
    public sealed class SQL_Query : mServerTask
    {
        public SqlCommand Cmd { get; private set; }

        public SQL_Query(SqlCommand cmd)
        {
            Cmd = cmd;
            OnLeave += SQL_Query_OnLeave;
        }

        private void SQL_Query_OnLeave(GameTime Now)
        {
            using (DatabaseClient pClient = DB.GetDatabaseClient())
            {
                try
                {
                    pClient.ExecuteNonQuery(Cmd);

                }
                catch (SqlException ex)
                {
                    DatabaseLog.Write(DatabaseLogLevel.Error, "Failed to Execute Query {0} {1}", Cmd.CommandText, ex.Message);
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
