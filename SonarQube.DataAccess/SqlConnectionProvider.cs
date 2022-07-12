using System.Configuration;

namespace SonarQube.DataAccess
{
    public static class SqlConnectionProvider
    {
        public static string GetSCAConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SCAConnectionString"].ConnectionString;
        }
    }
}
