using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hos.ScheduleMaster.Web.Filters;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Web.Extension;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class TaskController : BaseController
    {
        public static string ApiHost => System.Configuration.ConfigurationManager.AppSettings["TaskApiHost"];

        [Ninject.Inject]
        public Core.Service.ITaskService _taskService { get; set; }

        /// <summary>
        /// 任务列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 创建任务页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 编辑任务页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            return View();
        }

        /// <summary>
        /// 日志列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Log()
        {
            return View();
        }

        /// <summary>
        /// 清理日志页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearLog()
        {
            List<SelectListItem> selectData = new List<SelectListItem>();
            selectData.Add(new SelectListItem() { Text = "系统日志", Value = "0" });
            selectData.AddRange(_taskService.QueryAll().Select(row => new SelectListItem
            {
                Text = row.Title,
                Value = row.Id.ToString(),
                Selected = false
            }));
            ViewBag.TaskList = selectData;
            return View();
        }

        /// <summary>
        /// 清理日志
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnly]
        public ActionResult ClearLog(Guid? task, int? category, DateTime? startdate, DateTime? enddate)
        {
            var result = _taskService.DeleteLog(task, category, startdate, enddate);
            if (result > 0)
            {
                return SuccessTip($"清理成功！本次清理【{result}】条");
            }
            return DangerTip("没有符合条件的记录！");
        }
    }
}