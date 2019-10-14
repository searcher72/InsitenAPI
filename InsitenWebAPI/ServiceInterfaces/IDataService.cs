using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace InsitenWebAPI.ServiceInterfaces
{
    public interface IDataService
    {
        Task<DataTable> GetDataFromDatabase(string tableName, List<string> columnList, string filter, string orderBy, string resultDataTableName);
    }
}
