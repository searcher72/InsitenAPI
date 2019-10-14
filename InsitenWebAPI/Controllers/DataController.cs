using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InsitenAPI.Models;
using InsitenWebAPI.ServiceInterfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InsitenWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Data")]
    public class DataController : BaseController
    {
        // Log4Net initializer
        private static readonly ILog _log = LogManager.GetLogger(typeof(DataController));
        private IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [Route("Test")]
        public async Task<IActionResult> Test()
        {
            var test = Task.Run(() => ReturnStr());
            
            return Ok(test.Result);
        }

        private string ReturnStr()
        {
            return "test ok - 5";
        }

        /// <summary>
        /// Get data from database using the parameters provided
        /// </summary>
        /// <param name="body">Onjectt of type GetDataRequest</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetData")]
        public async Task<IActionResult> GetData([FromBody] JObject body)
        {
            var stopWatch = Stopwatch.StartNew();
            string methodName = "Get Data";
            GetDataRequest request = new GetDataRequest();
            var parameterList = string.Empty;

            try
            {
                request = body.ToObject<GetDataRequest>();
                parameterList = JsonConvert.SerializeObject(request);
                
                if(parameterList == null)
                {
                    return BadRequest(LogBadRequest(methodName, "Invalid json format!", parameterList, stopWatch.ElapsedMilliseconds));
                }

                // If parameters provided are not valid, send a bad request
                string validationText = IsValidParameters(request.TableName, request.ColumnList, request.Filter, request.OrderBy);
                if (!String.IsNullOrEmpty(validationText))
                {
                    return BadRequest(LogBadRequest(methodName, validationText, parameterList, stopWatch.ElapsedMilliseconds));
                }

                var dataTable = await _dataService.GetDataFromDatabase(request.TableName, request.ColumnList, request.Filter
                                            , request.OrderBy, request.DataTableName);

                // If no data is returned send a 204 message 
                if (dataTable == null || dataTable.Rows.Count <= 0)
                { 
                    return Ok(LogNotFoundRequest(methodName, parameterList, stopWatch.ElapsedMilliseconds));
                }
                else
                {
                    var json = JsonConvert.SerializeObject(dataTable);
                    LogSuccessfulGETRequest(methodName, json, parameterList, stopWatch.ElapsedMilliseconds);

                    return Ok(dataTable);
                }
            }
            catch (Exception ex)
            {
                string innerException = ex.InnerException == null ? "N/A" : ex.InnerException.Message;
                return StatusCode(500, LogServerErrorRequest(methodName, ex.Message, innerException, parameterList, stopWatch.ElapsedMilliseconds));
            }
        }

        /// <summary>
        /// Validate the inputs
        /// </summary>
        /// <param name="tableName">Valid table name contains only letters, numbers and _</param>
        /// <param name="columnList">Valid column name contains only letters, numbers and _</param>
        /// <param name="filter">Valid table name contains only letters, numbers and _ = ! spaces, less and greater than</param>
        /// <param name="orderBy">Valid column name contains only letters, numbers _ , and spaces </param>
        /// <returns></returns>
        private string IsValidParameters(string tableName, List<string> columnList, string filter, string orderBy)
        {
            string result = string.Empty;
            string pattern = @"^[a-zA-Z0-9_]*$";
            if (String.IsNullOrEmpty(tableName))
            {
                result = "Table name must be supplied.";
            }
            if(!Regex.IsMatch(tableName, pattern))
            {
                result = "Table name contains invalid characters";
            }
            foreach(var colName in columnList)
            {
                if (!Regex.IsMatch(colName, pattern))
                {
                    result = "Column name contains invalid characters";
                }
            }
            pattern = @"^[a-zA-Z0-9_, ]*$";
            if (!Regex.IsMatch(orderBy, pattern))
            {
                result = "Order by contains invalid characters";
            }
            pattern = @"^[a-zA-Z0-9_=!>< ]*$";
            if (!Regex.IsMatch(filter, pattern))
            {
                result = "Filter criteria contains invalid characters";
            }
            
            return result;
        }
    }
}