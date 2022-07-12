using System;
using System.Data;
using System.Data.SqlClient;
using NLog;

namespace SonarQube.DataAccess
{
    public static class ActivityDA
    {
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public static bool AddStatistics(DataTable warnings, DataTable securityhotspots, DataTable metricswithvalues)
		{
			bool result = false;
			try
			{
				SqlParameter[] commandParameters = new SqlParameter[3]
				{
				new SqlParameter
				{
					SqlDbType = SqlDbType.Structured,
					ParameterName = "@warnings",
					Value = warnings
				},
				new SqlParameter
				{
					SqlDbType = SqlDbType.Structured,
					ParameterName = "@securityhotspots",
					Value = securityhotspots
				},
				new SqlParameter
				{
					SqlDbType = SqlDbType.Structured,
					ParameterName = "@metricswithvalues",
					Value = metricswithvalues
				}
				};
				SqlHelper.ExecuteNonQuery(SqlConnectionProvider.GetSCAConnectionString(), CommandType.StoredProcedure, "SCA_AddStaistics", commandParameters);
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				return result;
			}
		}
	}
}
