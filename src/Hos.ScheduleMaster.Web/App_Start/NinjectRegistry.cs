using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Hos.ScheduleMaster.Core.Service;

namespace Hos.ScheduleMaster.Web
{
    public class NinjectRegistry
    {
        public static void Register(HttpConfiguration config)
        {
            Ninject.IKernel kernel = new Ninject.StandardKernel();

            kernel.Bind<Core.Repository.IUnitOfWork>().To<Core.EntityFramework.TaskDbContext>();

            //service binding
            //kernel.Bind<Core.Service.IAccountService>().To<Core.Service.AccountService>();
            kernel.Bind<Core.Service.ITaskService>().To<Core.Service.TaskService>();

            //mvc inject
            DependencyResolver.SetResolver(new MvcDependencyResolver(kernel));
            //webapi inject
            config.DependencyResolver = new WebApiDependencyResolver(kernel);

        }
    }
}