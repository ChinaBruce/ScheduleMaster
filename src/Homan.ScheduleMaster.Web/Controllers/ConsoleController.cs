using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Homan.ScheduleMaster.Web.Filters;

namespace Homan.ScheduleMaster.Web.Controllers
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