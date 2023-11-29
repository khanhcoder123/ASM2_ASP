using Microsoft.Data.SqlClient;

namespace Tranning
{
    public class DatabaseConnection
    {
        public static string GetStrConnection()
        {
            string strConnection = @"Data Source=DESKTOP-7ND6BBP;Initial Catalog=Tranning;Integrated Security=True;TrustServerCertificate=True;";
            return strConnection;
        }

        public static SqlConnection GetSqlConnection()
        {
            string strConnection = DatabaseConnection.GetStrConnection();
            SqlConnection connection = new SqlConnection(strConnection);
            return connection;
        }
    }
}
