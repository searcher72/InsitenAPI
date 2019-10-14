using InsitenAPI.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace InsitenWebAPI.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Private variable for log4net logging
        /// </summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof(BaseController));

        public BaseController()
        {   
        }

        #region Logging
        internal ErrMsg LogBadRequest(string methodName, string errorMsg, string parameterList, long timeExpired)
        {
            ErrMsg error = new ErrMsg();
            error.ErrorCode = "400";
            error.ErrorMessage = errorMsg;

            _log.Error(string.Format("{0}||failed!||{1}||{2}||{3}||400 Bad Request", methodName, parameterList, errorMsg, timeExpired));

            return error;
        }

        internal ErrMsg LogNotFoundRequest(string methodName, string parameterList, long timeExpired)
        {
            ErrMsg error = new ErrMsg();
            error.ErrorCode = "200";
            error.ErrorMessage = "No data returned!";

            _log.Error(string.Format("{0}||no data returned!||{1}||{2}||{3}||204 No Content", methodName, parameterList, "No data returned!", timeExpired));

            return error;
        }

        internal ErrMsg LogServerErrorRequest(string methodName, string errorMsg, string innerException, string parameterList, long timeExpired)
        {
            ErrMsg error = new ErrMsg();
            error.ErrorCode = "500";
            error.ErrorMessage = errorMsg.Contains("AX Integration") ? errorMsg : "A server error has occurred! Please try again. If the error persists, please contact the help desk!";

            _log.Error(string.Format("{0}||failed!||{1}||{2}:{3}||{4}||500 Server Error", methodName, parameterList, errorMsg,
                innerException, timeExpired));

            return error;
        }

        internal void LogSuccessfulGETRequest(string methodName, string jsonObj, string parameterList, long timeExpired)
        {
            _log.Info(string.Format("{0}||returned data!||{1}||{2}||{3}||200 OK", methodName, parameterList, jsonObj, timeExpired));
        }

        internal SuccessMsg LogSuccessfulPOSTRequest(string methodName, string parameterList, long timeExpired)
        {
            SuccessMsg success = new SuccessMsg();
            success.StatusCode = 0;
            success.Message = string.Format("{0} was successful!", methodName);

            _log.Info(string.Format("{0}||succeeded!||{1}||Post Succeeded||{2}||200 OK", methodName, parameterList, timeExpired)); //should be 204 for update however Boomi requires 200 for compliance.

            return success;
        }

        #endregion Logging
    }
}
