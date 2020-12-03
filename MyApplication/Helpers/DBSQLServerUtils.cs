using System.Data.SqlClient;

namespace WebApplication1.Helpers
{
    public class DBSQLServerUtils
    {
        public static SqlConnection GetDBConnection()
        {
            //
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=12345
            //
            //string connString = @"Data Source=" + datasource + ";Initial Catalog=" + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;
            string connString = "Server=(local);Database=ad_interface;Integrated Security=SSPI";

            return new SqlConnection(connString);
        }
    }
}
