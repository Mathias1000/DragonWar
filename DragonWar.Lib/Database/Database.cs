using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Lib.Database
{
    /// <summary>
    /// Represents a storage database.
    /// </summary>
    public class Database
    {
        #region Fields
        private readonly string mName;
        private readonly uint mMinPoolSize;
        private readonly uint mMaxPoolSize;

        private readonly string dataSource;
        private readonly string dataCatalog;

        #endregion

        #region Properties
        /// <summary>
        /// The name of the database to connect to.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// The minimum connection pool size for the database.
        /// </summary>
        internal uint minPoolSize
        {
            get { return mMinPoolSize; }
        }
        /// <summary>
        /// The maximum connection pool size for the database.
        /// </summary>
        internal uint maxPoolSize
        {
            get { return mMaxPoolSize; }
        }


        internal string mDataSource
        {
            get { return dataSource; }
        }

        internal string mDataCatalog
        {
            get { return dataCatalog; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a Database instance with given details.
        /// </summary>
        /// <param name="sName">The name of the database.</param>
        /// <param name="minPoolSize">The minimum connection pool size for the database.</param>
        /// <param name="maxPoolSize"> The maximum connection pool size for the database.</param>
        public Database(string sName, uint minPoolSize, uint maxPoolSize)
        {
            if (sName == null || sName.Length == 0)
                throw new ArgumentException(sName);

            mName = sName;
            mMinPoolSize = minPoolSize;
            mMaxPoolSize = maxPoolSize;
        }

        public Database(string DataSource, string DataCatalog, uint minPoolSize, uint maxPoolSize)//mssql
        {
            if (DataSource == null || DataSource.Length == 0)
                throw new ArgumentException(DataSource);

            if (DataCatalog == null || DataCatalog.Length == 0)
                throw new ArgumentException(DataCatalog);

            dataCatalog = DataCatalog;
            dataSource = DataSource;
            mMinPoolSize = minPoolSize;
            mMaxPoolSize = maxPoolSize;
        }
        #endregion
    }
}
