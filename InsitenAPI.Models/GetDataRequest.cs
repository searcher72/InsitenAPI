using System;
using System.Collections.Generic;

namespace InsitenAPI.Models
{
    public class GetDataRequest
    {
        public string TableName { get; set; }
        public List<string> ColumnList { get; set; }
        public string Filter { get; set; }
        public string OrderBy { get; set; }
        public string DataTableName { get; set; }
    }
}