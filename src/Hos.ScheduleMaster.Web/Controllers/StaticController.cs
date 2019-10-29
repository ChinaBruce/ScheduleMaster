using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hos.ScheduleMaster.Web.Controllers
{
    /// <summary>
    /// 一些公共的静态页面
    /// </summary>
    public class StaticController : Controller
    {
        /// <summary>
        /// 404页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Page404()
        {
            return View();
        }
    }
}