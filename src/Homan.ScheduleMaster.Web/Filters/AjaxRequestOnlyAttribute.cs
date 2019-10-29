using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Homan.ScheduleMaster.Web.Filters
{
    /// <summary>
    /// 异步请求特性
    /// by hoho
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AjaxRequestOnlyAttribute : ActionFilterAttribute
    {
        public AjaxRequestOnlyAttribute()
        {
            this.Order = 1;
        }

        /// <summary>在执行操作方法之前由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new RedirectResult("~/Static/Page404");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}