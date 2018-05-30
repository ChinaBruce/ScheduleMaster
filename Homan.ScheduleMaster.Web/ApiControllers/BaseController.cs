using Homan.ScheduleMaster.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Homan.ScheduleMaster.Web.ApiControllers
{
    public class BaseController : ApiController
    {
        /// <summary>
        /// 接口统一的返回消息
        /// </summary>
        /// <param name="result">返回内容</param>
        /// <returns></returns>
        protected HttpResponseMessage ApiResponse(ApiResponseMessage result)
        {
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new ObjectContent<ApiResponseMessage>(result, Configuration.Formatters.JsonFormatter),
            };
            return response;
        }

        protected HttpResponseMessage ApiResponse(ResultStatus status, string message, object data = null)
        {
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new ObjectContent<ApiResponseMessage>(new ApiResponseMessage(status, message, data), Configuration.Formatters.JsonFormatter),
            };
            return response;
        }

        protected HttpResponseMessage ApiResponse(object data = null)
        {
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new ObjectContent<object>(data, Configuration.Formatters.JsonFormatter),
            };
            return response;
        }
    }
}
