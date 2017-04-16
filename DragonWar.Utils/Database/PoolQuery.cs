using System.Data.SqlClient;


namespace DragonWar.Utils.Database
{
    public sealed class PoolQuery
    {
        public SqlCommand pCmd { get; private set; }

        public PoolQuery(SqlCommand cmd)
        {
            pCmd = cmd;
           
        }

    }
}
