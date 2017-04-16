using System.Text;
using DragonWar.Utils.ServerTask;
using DragonWar.Utils.Database;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using DragonWar.Utils.Core;

public class DatabaseManager : mServerTask
{

    private Dictionary<int, DatabaseClient> mClients = new Dictionary<int, DatabaseClient>();
    private int mStarvationCounter;
   
    private int mClientIdGenerator;
    private object mSyncRoot;

    private DatabaseServer mServer;
    private Database mDatabase;

    public override void Dispose()
    {
        mServer = null;
        mDatabase = null;
    }
    public int ClientCount
    {
        get
        {
            return mClients.Count;
        }
    }

    public DatabaseManager() { }
    internal DatabaseManager(DatabaseServer pServer, Database pDatabase)
    {
        mServer = pServer;
        mDatabase = pDatabase;



        mSyncRoot = new object();
        base.Intervall = (ServerTaskTimes)(mDatabase.ClientLifeTime*1000 / 2);
    }
    #region Util Function

    public string BuildConnectionString()
    {
        string cb = new SqlConnectionStringBuilder()
        {
            DataSource = mServer.Host,
            UserID = mServer.User,
            Password = mServer.Password,
            InitialCatalog = mDatabase.Name,
            MultipleActiveResultSets = true,
            IntegratedSecurity = false,
            MinPoolSize = mDatabase.minPoolSize,
            MaxPoolSize = mDatabase.maxPoolSize,
        }.ToString();

        return cb;
    }

    public bool TestConnection()
    {
        try
        {
            using (var connection = new SqlConnection(BuildConnectionString()))
            {
                connection.Open();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
    public  void PokeAllAwaiting()
    {
        Monitor.PulseAll(mSyncRoot);
    }
    private int GenerateClientId()
    {
        lock (mSyncRoot)
        {
            return mClientIdGenerator++;
        }
    }

    private  DatabaseClient CreateClient(int Id)
    {
        SqlConnection Connection = new SqlConnection(BuildConnectionString());
        Connection.Open();

        return new DatabaseClient(Id, Connection,this);
    }
    public override bool Update(GameTime Now)
    {
       
        if (ClientCount > mDatabase.minPoolSize)
        {
            lock (mSyncRoot)
            {
                List<int> ToDisconnect = new List<int>();

                foreach (DatabaseClient Client in mClients.Values)
                {
                    if (Client.Available && Client.TimeInactive >= mDatabase.ClientLifeTime)
                    {
                        ToDisconnect.Add(Client.Id);
                    }
                }

                foreach (int DisconnectId in ToDisconnect)
                {
                    mClients[DisconnectId].Close();
                    mClients.Remove(DisconnectId);
                }

                if (ToDisconnect.Count > 0)
                {
                    DatabaseLog.Write(DatabaseLogLevel.Debug, "(Sql)Disconnected " + ToDisconnect.Count + " inactive client(s).");
 
                }
                Monitor.PulseAll(mSyncRoot);
            }
        }
        return true;

    }

    public void SetClientAmount(int ClientAmount, string LogReason = "Unknown")
    {
        int Diff;

        lock (mSyncRoot)
        {
            Diff = ClientAmount - ClientCount;

            if (Diff > 0)
            {
                for (int i = 0; i < Diff; i++)
                {
                    int NewId = GenerateClientId();
                    mClients.Add(NewId, CreateClient(NewId));
                }
            }
            else
            {
                int ToDestroy = -Diff;
                int Destroyed = 0;

                foreach (DatabaseClient Client in mClients.Values)
                {
                    if (!Client.Available)
                    {
                        continue;
                    }

                    if (Destroyed >= ToDestroy || ClientCount <= mDatabase.minPoolSize)
                    {
                        break;
                    }

                    Client.Close();
                    mClients.Remove(Client.Id);
                    Destroyed++;
                }
            }
        }

        DatabaseLog.Write(DatabaseLogLevel.Debug, "(Sql) Client availability: " + ClientAmount + "; modifier: " + Diff + "; reason: "
            + LogReason + ".");
    }

    public DatabaseClient GetClient()
    {
        lock (mSyncRoot)
        {
            foreach (DatabaseClient Client in mClients.Values)
            {
                if (!Client.Available)
                {
                    continue;
                }

                DatabaseLog.Write(DatabaseLogLevel.Debug,"(Sql) Assigned client " + Client.Id + ".");
          
                if (!Client.CheckConnection())
                {
                    Client.Dispose();
                    return GetClient();
                }

                Client.Available = false;
                return Client;
            }

            if (mDatabase.maxPoolSize <= 0 || ClientCount < mDatabase.maxPoolSize) // Max pool size ignored if set to 0 or lower
            {
                SetClientAmount(ClientCount + 1, "out of assignable clients in GetClient()");
                return GetClient();
            }

            mStarvationCounter++;

            DatabaseLog.Write(DatabaseLogLevel.Warning, "(Sql) Client starvation; out of assignable clients/maximum pool size reached. Consider increasing the `mysql.pool.max` configuration value. Starvation count is " + mStarvationCounter + ".");

            // Wait until an available client returns
            Monitor.Wait(mSyncRoot);
            return GetClient();
        }
    }


    public void RunSQL(string sql, params SqlParameter[] Parameters)
    {
        StringBuilder sqlString = new StringBuilder();
        // Fix for floating point problems on some languages
        sqlString.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, sql, Parameters);

        SqlCommand sqlCommand = null;
        try
        {
            sqlCommand = new SqlCommand(sqlString.ToString());
            sqlCommand.Parameters.AddRange(Parameters);

            //Adding to ThreadPool :)
            ServerMainBase.InternalInstance.AddTask(new SQL_Query(sqlCommand));

        }
        catch (SqlException ex)
        {
            EngineLog.Write(ex, "Failed to Cache  Query {0}", sqlCommand.CommandText);
        }
    }

    public SQLResult Select(string sql, params SqlParameter[] Parameters)
    {


        using (DatabaseClient pClient = GetClient())
        {

            StringBuilder sqlString = new StringBuilder();
            // Fix for floating point problems on some languages
            sqlString.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, sql, Parameters);
  
            SqlCommand sqlCommand = new SqlCommand(sqlString.ToString());
            
            try
            {
                sqlCommand.Parameters.AddRange(Parameters);

                using (var SqlData = pClient.ExecuteReader(sqlCommand))
                {
                    using (var retData = new SQLResult())
                    {
                        retData.Load(SqlData);
                        retData.Count = retData.Rows.Count;

                        return retData;
                    }
                }
            }
            catch (SqlException ex)
            {
                DatabaseLog.Write(ex, "Error With Query {0}", sqlCommand.CommandText);
                return null;
            }
        }
    }
    #endregion

}