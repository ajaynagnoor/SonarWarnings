using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace SonarQube.DataAccess
{
    public sealed class SqlHelperParameterCache
	{
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

		private SqlHelperParameterCache()
		{
		}

		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			string key = connectionString + ":" + commandText;
			paramCache[key] = commandParameters;
		}

		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			string key = connectionString + ":" + commandText;
			SqlParameter[] array = paramCache[key] as SqlParameter[];
			if (array != null)
			{
				return CloneParameters(array);
			}
			return null;
		}

		public static SqlParameter[] GetSpParameterSet(string connectionString, string procedure)
		{
			return GetSpParameterSet(connectionString, procedure, includeReturnValueParameter: false);
		}

		public static SqlParameter[] GetSpParameterSet(string connectionString, string procedure, bool includeReturnValueParameter)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (procedure == null || procedure.Length == 0)
			{
				throw new ArgumentNullException("procedure");
			}
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				return GetSpParameterSetInternal(connection, procedure, includeReturnValueParameter);
			}
		}

		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string procedure)
		{
			return GetSpParameterSet(connection, procedure, includeReturnValueParameter: false);
		}

		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string procedure, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			using (SqlConnection connection2 = (SqlConnection)((ICloneable)connection).Clone())
			{
				return GetSpParameterSetInternal(connection2, procedure, includeReturnValueParameter);
			}
		}

		private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string procedure, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (procedure == null || procedure.Length == 0)
			{
				throw new ArgumentNullException("procedure");
			}
			string key = connection.ConnectionString + ":" + procedure + (includeReturnValueParameter ? ":include ReturnValue Parameter" : string.Empty);
			SqlParameter[] array = paramCache[key] as SqlParameter[];
			if (array == null)
			{
				SqlParameter[] array2 = DiscoverSpParameterSet(connection, procedure, includeReturnValueParameter);
				paramCache[key] = array2;
				array = array2;
			}
			return CloneParameters(array);
		}

		private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string procedure, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (procedure == null || procedure.Length == 0)
			{
				throw new ArgumentNullException("procedure");
			}
			SqlCommand sqlCommand = new SqlCommand(procedure, connection)
			{
				CommandType = CommandType.StoredProcedure
			};
			connection.Open();
			SqlCommandBuilder.DeriveParameters(sqlCommand);
			connection.Close();
			if (!includeReturnValueParameter)
			{
				sqlCommand.Parameters.RemoveAt(0);
			}
			SqlParameter[] array = new SqlParameter[sqlCommand.Parameters.Count];
			sqlCommand.Parameters.CopyTo(array, 0);
			SqlParameter[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Value = DBNull.Value;
			}
			return array;
		}

		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] array = new SqlParameter[originalParameters.Length];
			int i = 0;
			for (int num = originalParameters.Length; i < num; i++)
			{
				array[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
			}
			return array;
		}
	}
}
