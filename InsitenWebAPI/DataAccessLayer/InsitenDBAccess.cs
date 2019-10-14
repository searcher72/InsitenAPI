using InsitenWebAPI.DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace InsitenWebAPI.DataAccessLayer.Methods
{
    public class InsitenDBAccess : IInsitenDBAccess
    {
        private readonly InsitenDBContext _context;
        private const int _timeOut = 30;

        public InsitenDBAccess(InsitenDBContext context)
        {
            _context = context;
        }

        #region Properties
        public DbCommand Command { get; set; }

        public DbParameterCollection Parameters
        {
            get { return Command.Parameters; }
        }

        public DbTransaction Transaction { get; set; }

        public object OutputParameterValue(string parameterName)
        {
            return Command.Parameters[string.Format("{0}", parameterName)].Value;
        }

        #endregion

        #region Connection
        /// <summary>
        /// Get connection to database
        /// </summary>
        public void InitializeConnection()
        {
            if (_context.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _context.Database.OpenConnection();
            }
        }

        /// <summary>
        /// Close connection to database
        /// </summary>
        public void CloseConnection()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }

            if (_context.Database.GetDbConnection().State == ConnectionState.Open)
            {
                _context.Database.CloseConnection();
            }
        }

        /// <summary>
        /// Start transaction to database
        /// </summary>
        public void CreateTransaction()
        {
            if (Transaction == null)
            {
                Transaction = _context.Database.GetDbConnection().BeginTransaction(IsolationLevel.ReadUncommitted);
            }
        }

        /// <summary>
        /// Close connection to database
        /// </summary>
        public void CommitTransaction()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
            }
        }

        /// <summary>
        /// Rollback transaction in case of issue
        /// </summary>
        public void RollbackTransaction()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
        }

        /// <summary>
        /// Initialize command for using stored procedure
        /// </summary>
        public void CreateCommand()
        {
            Command = _context.Database.GetDbConnection().CreateCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.Transaction = Transaction != null ? Transaction : null;
        }

        #endregion

        /// <summary>
        /// Add SQL Parameter to command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="direction"></param>
        public void AddParameter(string name, object value, SqlDbType sqlDbType, ParameterDirection direction)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.Value = value == null ? (object)DBNull.Value : value;
            parameter.SqlDbType = sqlDbType;
            parameter.Direction = direction;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Add SQL Parameter to command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        public void AddParameter(string name, object value, SqlDbType sqlDbType, int size, ParameterDirection direction)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.Value = value == null ? (object)DBNull.Value : value;
            parameter.SqlDbType = sqlDbType;
            parameter.Size = size;
            parameter.Direction = direction;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Add SQL Parameter to command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="direction"></param>
        /// <param name="typeName"></param>
        public void AddParameter(string name, object value, SqlDbType sqlDbType, ParameterDirection direction, string typeName)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.Value = value == null ? (object)DBNull.Value : value;
            parameter.SqlDbType = sqlDbType;
            parameter.Direction = direction;
            parameter.TypeName = typeName;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Add SQL Parameter to command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="direction"></param>
        public void AddParameter(string name, SqlDbType sqlDbType, ParameterDirection direction)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = sqlDbType;
            parameter.Direction = direction;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Add SQL Parameter to command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        public void AddParameter(string name, SqlDbType sqlDbType, int size, ParameterDirection direction)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = sqlDbType;
            parameter.Size = size;
            parameter.Direction = direction;

            Command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Clear SQL Parameters from command
        /// </summary>
        public void ClearParameters()
        {
            Command.Parameters.Clear();
            Command.CommandText = string.Empty;
        }

        /// <summary>
        /// ExecuteNonQueryAsync
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public async Task<int> ExecStoredProc(string procName)
        {
            Command.CommandText = procName;
            Command.CommandTimeout = _timeOut;
            return await Command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// ExecuteReaderAsync
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public async Task<DbDataReader> GetDataReader(string procName)
        {
            Command.CommandText = procName;
            Command.CommandTimeout = _timeOut;
            return await Command.ExecuteReaderAsync();
        }

        /// <summary>
        /// Execute stored proc and return result as DataTable
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="tableName">Name for the returned datatabale</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> GetDataTableAsync(string procName, string tableName = null)
        {
            Command.CommandText = procName;
            Command.CommandTimeout = _timeOut;

            TaskCompletionSource<DataTable> source = new TaskCompletionSource<DataTable>();
            var resultTable = new DataTable(tableName ?? Command.CommandText);
            DbDataReader dataReader = null;

            try
            {
                dataReader = await Command.ExecuteReaderAsync(CommandBehavior.Default);
                resultTable.Load(dataReader);
                source.SetResult(resultTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                Command.Connection.Close();
            }

            return resultTable;
        }

        /// <summary>
        /// Dispose DbCommand
        /// </summary>
        public void DisposeCommand()
        {
            if (Command != null)
            {
                Command.Dispose();
            }
        }
    }
}