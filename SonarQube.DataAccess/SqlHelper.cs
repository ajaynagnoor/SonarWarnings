using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Xml;

namespace SonarQube.DataAccess
{
    public sealed class SqlHelper
    {
        private enum SqlConnectionOwnership
        {
            Internal,
            External
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                return ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
            }
        }

        public static int ExecuteNonQuery(string connectionString, string procedure, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out bool mustCloseConnection);
            int result = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }
            return result;
        }

        public static int ExecuteNonQuery(SqlConnection connection, string procedure, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteNonQuery(connection, CommandType.StoredProcedure, procedure);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out bool _);
            int result = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return result;
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string procedure, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, procedure);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                return ExecuteDataset(sqlConnection, commandType, commandText, commandParameters);
            }
        }

        public static DataSet ExecuteDataset(string connectionString, string procedure, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteDataset(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out bool mustCloseConnection);
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
            {
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                sqlCommand.Parameters.Clear();
                if (mustCloseConnection)
                {
                    connection.Close();
                }
                return dataSet;
            }
        }

        public static DataSet ExecuteDataset(SqlConnection connection, string procedure, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteDataset(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteDataset(connection, CommandType.StoredProcedure, procedure);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out bool _);
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
            {
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                sqlCommand.Parameters.Clear();
                return dataSet;
            }
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, string procedure, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteDataset(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteDataset(transaction, CommandType.StoredProcedure, procedure);
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                return ExecuteReader(sqlConnection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                sqlConnection?.Close();
                throw;
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, int cmdTimeOut, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                return ExecuteReader(sqlConnection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal, cmdTimeOut);
            }
            catch
            {
                sqlConnection?.Close();
                throw;
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString, string procedure, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteReader(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteReader(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, string procedure, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteReader(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteReader(connection, CommandType.StoredProcedure, procedure);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string procedure, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteReader(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteReader(transaction, CommandType.StoredProcedure, procedure);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                return ExecuteScalar(sqlConnection, commandType, commandText, commandParameters);
            }
        }

        public static object ExecuteScalar(string connectionString, string procedure, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteScalar(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out bool mustCloseConnection);
            object result = sqlCommand.ExecuteScalar();
            sqlCommand.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }
            return result;
        }

        public static object ExecuteScalar(SqlConnection connection, string procedure, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteScalar(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteScalar(connection, CommandType.StoredProcedure, procedure);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out bool _);
            object result = sqlCommand.ExecuteScalar();
            sqlCommand.Parameters.Clear();
            return result;
        }

        public static object ExecuteScalar(SqlTransaction transaction, string procedure, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteScalar(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteScalar(transaction, CommandType.StoredProcedure, procedure);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            bool mustCloseConnection = false;
            SqlCommand sqlCommand = new SqlCommand();
            try
            {
                PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
                XmlReader result = sqlCommand.ExecuteXmlReader();
                sqlCommand.Parameters.Clear();
                return result;
            }
            catch
            {
                if (mustCloseConnection)
                {
                    connection.Close();
                }
                throw;
            }
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, string procedure, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteXmlReader(connection, CommandType.StoredProcedure, procedure);
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out bool _);
            XmlReader result = sqlCommand.ExecuteXmlReader();
            sqlCommand.Parameters.Clear();
            return result;
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string procedure, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, procedure);
        }

        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                FillDataset(sqlConnection, commandType, commandText, dataSet, tableNames);
            }
        }

        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                FillDataset(sqlConnection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        public static void FillDataset(string connectionString, string procedure, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                FillDataset(sqlConnection, procedure, dataSet, tableNames, parameterValues);
            }
        }

        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, (SqlParameter[])null);
        }

        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(SqlConnection connection, string procedure, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                FillDataset(connection, CommandType.StoredProcedure, procedure, dataSet, tableNames, spParameterSet);
            }
            else
            {
                FillDataset(connection, CommandType.StoredProcedure, procedure, dataSet, tableNames);
            }
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, (SqlParameter[])null);
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(SqlTransaction transaction, string procedure, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (parameterValues != null && parameterValues.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, parameterValues);
                FillDataset(transaction, CommandType.StoredProcedure, procedure, dataSet, tableNames, spParameterSet);
            }
            else
            {
                FillDataset(transaction, CommandType.StoredProcedure, procedure, dataSet, tableNames);
            }
        }

        public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (tableName == null || tableName.Length == 0)
            {
                throw new ArgumentNullException("tableName");
            }
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
            {
                UpdateCommand = (updateCommand ?? throw new ArgumentNullException("updateCommand")),
                InsertCommand = (insertCommand ?? throw new ArgumentNullException("insertCommand")),
                DeleteCommand = (deleteCommand ?? throw new ArgumentNullException("deleteCommand"))
            })
            {
                sqlDataAdapter.Update(dataSet, tableName);
                dataSet.AcceptChanges();
            }
        }

        public static SqlCommand CreateCommand(SqlConnection connection, string procedure, params string[] sourceColumns)
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
            if (sourceColumns != null && sourceColumns.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                for (int i = 0; i < sourceColumns.Length; i++)
                {
                    spParameterSet[i].SourceColumn = sourceColumns[i];
                }
                AttachParameters(sqlCommand, spParameterSet);
            }
            return sqlCommand;
        }

        public static int ExecuteNonQueryTypedParams(string connectionString, string procedure, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static int ExecuteNonQueryTypedParams(SqlConnection connection, string procedure, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteNonQuery(connection, CommandType.StoredProcedure, procedure);
        }

        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, string procedure, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, procedure);
        }

        public static DataSet ExecuteDatasetTypedParams(string connectionString, string procedure, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteDataset(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, string procedure, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteDataset(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteDataset(connection, CommandType.StoredProcedure, procedure);
        }

        public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, string procedure, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteDataset(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteDataset(transaction, CommandType.StoredProcedure, procedure);
        }

        public static SqlDataReader ExecuteReaderTypedParams(string connectionString, string procedure, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteReader(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteReader(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, string procedure, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteReader(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteReader(connection, CommandType.StoredProcedure, procedure);
        }

        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, string procedure, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteReader(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteReader(transaction, CommandType.StoredProcedure, procedure);
        }

        public static object ExecuteScalarTypedParams(string connectionString, string procedure, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0)
            {
                throw new ArgumentNullException("connectionString");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteScalar(connectionString, CommandType.StoredProcedure, procedure);
        }

        public static object ExecuteScalarTypedParams(SqlConnection connection, string procedure, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteScalar(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteScalar(connection, CommandType.StoredProcedure, procedure);
        }

        public static object ExecuteScalarTypedParams(SqlTransaction transaction, string procedure, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteScalar(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteScalar(transaction, CommandType.StoredProcedure, procedure);
        }

        public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string procedure, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteXmlReader(connection, CommandType.StoredProcedure, procedure);
        }

        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string procedure, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (transaction.Connection == null)
            {
                throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
            }
            if (procedure == null || procedure.Length == 0)
            {
                throw new ArgumentNullException("procedure");
            }
            if (dataRow != null && dataRow.ItemArray.Length != 0)
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, procedure);
                AssignParameterValues(spParameterSet, dataRow);
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, procedure, spParameterSet);
            }
            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, procedure);
        }

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandParameters == null)
            {
                throw new ArgumentNullException("commandParameters");
            }
            foreach (SqlParameter sqlParameter in commandParameters)
            {
                if (sqlParameter != null)
                {
                    if ((sqlParameter.Direction == ParameterDirection.InputOutput || sqlParameter.Direction == ParameterDirection.Input) && sqlParameter.Value == null)
                    {
                        sqlParameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(sqlParameter);
                }
            }
        }

        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if (commandParameters == null || dataRow == null)
            {
                return;
            }
            int num = 0;
            foreach (SqlParameter sqlParameter in commandParameters)
            {
                if (sqlParameter.ParameterName == null || sqlParameter.ParameterName.Length <= 1)
                {
                    throw new ArgumentNullException(string.Format(CultureInfo.InvariantCulture, "Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", num, sqlParameter.ParameterName));
                }
                if (dataRow.Table.Columns.IndexOf(sqlParameter.ParameterName.Substring(1)) != -1)
                {
                    sqlParameter.Value = dataRow[sqlParameter.ParameterName.Substring(1)];
                }
                num++;
            }
        }

        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if (commandParameters == null || parameterValues == null)
            {
                return;
            }
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }
            int i = 0;
            for (int num = commandParameters.Length; i < num; i++)
            {
                IDbDataParameter dbDataParameter = parameterValues[i] as IDbDataParameter;
                if (dbDataParameter != null)
                {
                    if (dbDataParameter.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = dbDataParameter.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException("commandText");
            }
            command.CommandTimeout = 1800;
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
                }
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        private static bool PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, int cmdTimeout)
        {
            bool result = false;
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException("commandText");
            }
            command.CommandTimeout = cmdTimeout;
            if (connection.State != ConnectionState.Open)
            {
                result = true;
                connection.Open();
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("This was rollbacked or commited, please provide it in an open state.", "transaction");
                }
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return result;
        }

        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            bool mustCloseConnection = false;
            SqlCommand sqlCommand = new SqlCommand();
            try
            {
                PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
                SqlDataReader result = (connectionOwnership == SqlConnectionOwnership.External) ? sqlCommand.ExecuteReader() : sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                bool flag = true;
                foreach (SqlParameter parameter in sqlCommand.Parameters)
                {
                    if (parameter.Direction != ParameterDirection.Input)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    sqlCommand.Parameters.Clear();
                }
                return result;
            }
            catch
            {
                if (mustCloseConnection)
                {
                    connection.Close();
                }
                throw;
            }
        }

        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership, int cmdTimeOut)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            bool flag = false;
            SqlCommand sqlCommand = new SqlCommand();
            try
            {
                flag = PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters, cmdTimeOut);
                SqlDataReader result = (connectionOwnership == SqlConnectionOwnership.External) ? sqlCommand.ExecuteReader() : sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                bool flag2 = true;
                foreach (SqlParameter parameter in sqlCommand.Parameters)
                {
                    if (parameter.Direction != ParameterDirection.Input)
                    {
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    sqlCommand.Parameters.Clear();
                }
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    connection.Close();
                }
            }
        }

        private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            SqlCommand sqlCommand = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
            {
                if (tableNames != null && tableNames.Length != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder("Table");
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        if (tableNames[i] == null || tableNames[i].Length == 0)
                        {
                            throw new ArgumentException("This parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        }
                        sqlDataAdapter.TableMappings.Add(stringBuilder.ToString(), tableNames[i]);
                        stringBuilder.Append(i + 1).ToString();
                    }
                }
                sqlDataAdapter.Fill(dataSet);
                sqlCommand.Parameters.Clear();
            }
            if (mustCloseConnection)
            {
                connection.Close();
            }
        }
    }
}
