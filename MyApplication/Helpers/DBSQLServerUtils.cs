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
            string connString = @"Data Source=OR180075\SQLEXPRESS;Initial Catalog=ad_interface;Integrated Security=True";

            return new SqlConnection(connString);
        }
    }
}
