using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Homan.ScheduleMaster.Web.Controllers
{
    public class LoginController : Controller
    {
        [Ninject.Inject]
        public Core.Service.IAccountService _accountService { get; set; }

        /// <summary>
        /// 登入页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录请求处理
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = _accountService.LoginCheck(username, password);
            if (user != null)
            {
                //序列化admin对象
                string accountJson = JsonConvert.SerializeObject(user);
                //创建用户票据
                var ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddDays(1), false, accountJson);
                //加密
                string encryptAccount = FormsAuthentication.Encrypt(ticket);
                //创建cookie
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptAccount)
                {
                    HttpOnly = true,
                    Secure = FormsAuthentication.RequireSSL,
                    Domain = FormsAuthentication.CookieDomain,
                    Path = FormsAuthentication.FormsCookiePath
                };
                cookie.Expires = DateTime.Now.AddDays(1);
                //写入Cookie
                Response.Cookies.Remove(cookie.Name);
                Response.Cookies.Add(cookie);
                return JavaScript($"$('#btnLogin').val('successed！redirecting...');location.href='{Url.Action("Index", "Console")}';");
            }
            return JavaScript("showTips('用户名或密码错误！');");
        }

        /// <summary>
        /// 注销登录状态
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty)
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(-1),
                Secure = FormsAuthentication.RequireSSL,
                Domain = FormsAuthentication.CookieDomain,
                Path = FormsAuthentication.FormsCookiePath
            };
            Response.Cookies.Remove(cookie.Name);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");

        }
    }
}