using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace InsitenWebAPI.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Interface for Data Access
    /// </summary>
    public interface IInsitenDBAccess
    {
        DbTransaction Transaction { get; set; }
        void InitializeConnection();
        void CloseConnection();
        void CreateTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void CreateCommand();
        void AddParameter(string name, object value, SqlDbType sqlDbType, ParameterDirection direction);
        void AddParameter(string name, object value, SqlDbType sqlDbType, int size, ParameterDirection direction);
        void AddParameter(string name, object value, SqlDbType sqlDbType, ParameterDirection direction, string typeName);
        void AddParameter(string name, SqlDbType sqlDbType, ParameterDirection direction);
        void AddParameter(string name, SqlDbType sqlDbType, int size, ParameterDirection direction);
        object OutputParameterValue(string parameterName);
        void ClearParameters();
        Task<int> ExecStoredProc(string procName);
        Task<DbDataReader> GetDataReader(string procName);
        Task<DataTable> GetDataTableAsync(string procName, string tableName = null);               
        void DisposeCommand();
    }
}