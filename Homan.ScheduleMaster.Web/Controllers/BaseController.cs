using Homan.ScheduleMaster.Base.Models;
using Homan.ScheduleMaster.Web.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Homan.ScheduleMaster.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string tokenName = FormsAuthentication.FormsCookieName;

        /// <summary>
        /// 返回数据表格的json数据
        /// </summary>
        /// <param name="total"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonNetResult DataGrid(int total, object data)
        {
            return this.JsonNet(new { total, rows = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回404页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PageNotFound()
        {
            return RedirectToAction("Page404", "Static");
        }

        #region 前端提示信息

        protected JavaScriptResult SuccessTip(string text, string redirect = "", string callback = "null")
        {
            return new JavaScriptResult() { Script = $"$tools.successTip('{text}','{redirect}',{callback});" };
        }

        protected JavaScriptResult DangerTip(string text, string redirect = "", string callback = "null")
        {
            return new JavaScriptResult() { Script = $"$tools.errorTip('{text}','{redirect}',{callback});" };
        }

        protected JavaScriptResult WarningTip(string text, string redirect = "", string callback = "null")
        {
            return new JavaScriptResult() { Script = $"$tools.warningTip('{text}','{redirect}',{callback});" };
        }

        #endregion

        /// <summary>
        /// 当前登陆的管理员
        /// </summary>
        public SystemUser CurrentAdmin
        {
            get
            {
                HttpCookie cookie = HttpContext.Request.Cookies[tokenName];
                if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                {
                    return null;
                }
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                SystemUser admin = JsonConvert.DeserializeObject<SystemUser>(ticket.UserData);
                return admin;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //增加登录token的有效时间
            HttpCookie token = Request.Cookies[tokenName];
            if (token != null)
            {
                var cookieToken = new HttpCookie(tokenName, token.Value)
                {
                    Expires = DateTime.Now.AddDays(1),
                    Secure = FormsAuthentication.RequireSSL,
                    Domain = FormsAuthentication.CookieDomain,
                    Path = FormsAuthentication.FormsCookiePath,
                    HttpOnly = true
                };
                Response.Cookies.Remove(cookieToken.Name);
                Response.Cookies.Add(cookieToken);
            }
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// 登录状态检查
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //return;
            var anonymousAction = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            if (!anonymousAction.Any())
            {
                var user = CurrentAdmin;
                if (user == null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var accept = filterContext.HttpContext.Request.AcceptTypes;
                        if (accept.Contains("application/json"))
                        {
                            filterContext.Result = new JsonNetResult()
                            {
                                Data = new { Success = false, Msg = "登录已超时！" },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                        {
                            filterContext.Result = new JavaScriptResult() { Script = "alert('登录已超时！')" };
                        }
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/Login/Index");
                    }
                }
            }
            //base.OnAuthorization(filterContext);
        }
    }
}