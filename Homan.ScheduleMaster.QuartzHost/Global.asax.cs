using Homan.ScheduleMaster.Core.Models;
using Homan.ScheduleMaster.Core.Utility;
using Homan.ScheduleMaster.Infrastructure.EntityFramework;
using Homan.ScheduleMaster.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Homan.ScheduleMaster.QuartzHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            SQLHelper.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConn"].ConnectionString;

            LogManager.Init();
            Common.QuartzManager.InitScheduler();

            using (TaskDbContext db = new TaskDbContext())
            {
                var list = db.Task.Where(m => m.Status == (int)TaskStatus.Running).ToList();
                foreach (var task in list)
                {
                    Common.QuartzManager.StartWithRetry(task);
                }
            }
        }

        protected void Application_End()
        {
            Common.QuartzManager.Shutdown();
            LogManager.Shutdown();
        }
    }
}
