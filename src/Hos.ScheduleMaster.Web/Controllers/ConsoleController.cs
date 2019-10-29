using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hos.ScheduleMaster.Web.Filters;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class ConsoleController : BaseController
    {
        // GET: Console
        public ActionResult Index()
        {
            ViewBag.CurrentAdmin = CurrentAdmin;
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }
    }
}