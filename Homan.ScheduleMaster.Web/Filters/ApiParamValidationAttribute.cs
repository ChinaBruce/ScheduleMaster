using Homan.ScheduleMaster.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Homan.ScheduleMaster.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ApiParamValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                string error = string.Empty;
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        var modelError = state.Errors.First();
                        if (modelError.Exception != null)
                        {
                            error = modelError.Exception.Message;
                        }
                        else
                        {
                            error = modelError.ErrorMessage;
                            if (error.Length < 1)
                            {
                                error = $"参数格式验证失败:[{key}]";
                            }
                        }
                        break;
                    }
                }
                ApiResponseMessage response = new ApiResponseMessage() { Status = ResultStatus.ParamError, Message = error };
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Accepted)
                {
                    Content = new ObjectContent<ApiResponseMessage>(response,
                        actionContext.ActionDescriptor.Configuration.Formatters.JsonFormatter)
                };
            }
        }
    }
}