using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using System.Data;

using MySql.Data.MySqlClient;

namespace DragonWar.Lib.Database
{
    /// <summary>
    /// Represents a client of a database,
    /// </summary>
    public sealed class DatabaseClient : IDisposable
    {
        #region Fields
        private uint mHandle;
        private DateTime mLastActivity;

        private DatabaseManager mManager;
        private MySqlConnection mConnection;
        private MySqlCommand mCommand;

        internal bool IsBussy = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the handle of this database client.
        /// </summary>
        internal uint Handle
        {
            get { return mHandle; }
        }
        /// <summary>
        /// Gets whether this database client is anonymous and does not recycle in the database manager.
        /// </summary>
        internal bool Anonymous
        {
            get { return (mHandle == 0); }
        }
        /// <summary>
        /// Gets the DateTime object representing the date and time this client has been used for the last time.
        /// </summary>
        //internal DateTime LastActivity
        //{
        //    get { return mLastActivity; }
        //}
        /// <summary>
        /// Gets the amount of seconds that this client has been inactive.
        /// </summary>
        internal int Inactivity
        {
            get { return (int)(DateTime.Now - mLastActivity).TotalSeconds; }
        }
        /// <summary>
        /// Gets the state of the connection instance.
        /// </summary>
        internal ConnectionState State
        {
            get { return (mConnection != null) ? mConnection.State : ConnectionState.Broken; }

        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new database client with a given handle to a given database proxy.
        /// </summary>
        /// <param name="Handle">The identifier of this database client as an unsigned 32 bit integer.</param>
        /// <param name="pManager">The instance of the DatabaseManager that manages the database proxy of this database client.</param>
        internal DatabaseClient(uint Handle, DatabaseManager pManager)
        {
            if (pManager == null)
                throw new ArgumentNullException("pManager");

            mHandle = Handle;
            mManager = pManager;

            mConnection = new MySqlConnection(mManager.CreateConnectionString());
            mCommand = mConnection.CreateCommand();

            UpdateLastActivity();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Attempts to open the database connection.
        /// </summary>
        internal void Connect()
        {
            if (mConnection == null)
                throw new DatabaseException("Connection instance of database client " + mHandle + " holds no value.");
            if (mConnection.State != ConnectionState.Closed)
                throw new DatabaseException("Connection instance of database client " + mHandle + " requires to be closed before it can open again.");

            try
            {
                mConnection.Open();
            }
            catch (MySqlException mex)
            {
                throw new DatabaseException("Failed to open connection for database client " + mHandle + ", exception message: " + mex.Message);
            }

        }
        /// <summary>
        /// Attempts to close the database connection.
        /// </summary>
        internal void Disconnect()
        {
            try
            {
                mConnection.Close();
            }
            catch { }
        }
        /// <summary>
        /// Closes the database connection (if open) and disposes all resources.
        /// </summary>
        internal void Destroy()
        {
            Disconnect();

            mConnection.Dispose();
            //mConnection = null;

            mCommand.Dispose();
            //mCommand = null;

            //mManager = null;
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Updates the last activity timestamp to the current date and time.
        /// </summary>
        internal void UpdateLastActivity()
        {
            mLastActivity = DateTime.Now;
        }


        #region IDisposable members
        /// <summary>
        /// Returns the DatabaseClient to the DatabaseManager, where the connection will stay alive for 30 seconds of inactivity.
        /// </summary>
        public void Dispose()
        {
            if (this.Anonymous == false) // No disposing for this client yet! Return to the manager!
            {
                IsBussy = false;
                //Reset this!
                mCommand.CommandText = null;
                mCommand.Parameters.Clear();

                mManager.ReleaseClient(mHandle);
            }
            else // Anonymous client, dispose this right away!
            {
                Destroy();
            }
        }
        #endregion



        #endregion

        #region Statements

       /*
    public  bool RunSQL(string sql, params MySqlParameter[] Parameters)
    {
        MySqlConnection Connection = null;
        //DatabaseManager.CheckConnection(ref Connection);
        StringBuilder sqlString = new StringBuilder();
        // Fix for floating point problems on some languages
        sqlString.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, sql, Parameters);

        MySqlCommand sqlCommand = null;
        try
        {
            sqlCommand = new MySqlCommand(sqlString.ToString(), Connection);
            sqlCommand.Parameters.AddRange(Parameters);
            sqlCommand.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex)
        {
         //   Log.WriteLine(LogLevel.Error, "Error With Query {0}", sqlCommand.CommandText);
            return false;
        }
    }
        
            public  SQLResult Select(string sql, params MySqlParameter[] Parameters)
            {

                MySqlConnection Connection = null;
              //  DatabaseManager.CheckConnection(ref Connection);
                StringBuilder sqlString = new StringBuilder();
                // Fix for floating point problems on some languages
                sqlString.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, sql, Parameters);

                MySqlCommand sqlCommand = new MySqlCommand(sqlString.ToString(), Connection);

                try
                {


                    sqlCommand.Parameters.AddRange(Parameters);

                    using (var SqlData = sqlCommand.ExecuteReader())
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

                    Log.WriteLine(LogLevel.Error, "Error With Query {0}", sqlCommand.CommandText);
                }

                return null;
            }
 */
    }
}
#endregion