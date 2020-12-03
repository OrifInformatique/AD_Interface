using System.Data.SqlClient;

namespace WebApplication1.Helpers
{
    public class DBUtils
    {
        public static SqlConnection GetDBConnection()
        {
            return DBSQLServerUtils.GetDBConnection();
        }
    }
}