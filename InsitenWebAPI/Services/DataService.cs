using InsitenWebAPI.DataAccessLayer.Interfaces;
using InsitenWebAPI.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InsitenWebAPI.Services
{
    /// <summary>
    /// Service class for DataController
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IInsitenDBAccess _db;
        public DataService(IInsitenDBAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get data from database based on supplied parameters
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="resultDataTableName"></param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> GetDataFromDatabase(string tableName, List<string> columnList, string filter, string orderBy, string resultDataTableName)
        {
            DataTable response = new DataTable();
            string procName = "GetDataFromDatabase";

            try
            {
                if(String.IsNullOrEmpty(resultDataTableName))
                {
                    resultDataTableName = "ResultData";
                }
                _db.InitializeConnection();
                _db.CreateCommand();
                _db.AddParameter("@sTableName", tableName, SqlDbType.VarChar, 100, ParameterDirection.Input);

                DataTable dt = new DataTable();
                dt.Columns.Add("ColumnName", typeof(string));
                foreach(string column in columnList)
                {
                    dt.Rows.Add(column);
                }
                _db.AddParameter("@sColumnList", dt, SqlDbType.Structured, ParameterDirection.Input, "dbo.ColumnListType");
                
                _db.AddParameter("@sFilter", filter, SqlDbType.VarChar, 1000, ParameterDirection.Input);
                _db.AddParameter("@sOrderBy", orderBy, SqlDbType.VarChar, 1000, ParameterDirection.Input);

                using (response = await _db.GetDataTableAsync(procName, resultDataTableName))
                {}
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("An error has occurred executing the following stored proc : {0}, error: {1}", procName, e.Message));
            }
            finally
            {
                _db.DisposeCommand();
                _db.CloseConnection();
            }

            return response;
        }
    }
}