using Homan.ScheduleMaster.Core.Models;
using Homan.ScheduleMaster.Web.Common;
using Homan.ScheduleMaster.Web.Dto;
using Homan.ScheduleMaster.Web.Filters;
using Homan.ScheduleMaster.Web.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Homan.ScheduleMaster.Web.ApiControllers
{
    [RoutePrefix("api/Task")] 
    public class TaskController : BaseController
    {
        private TaskService _taskService;

        public TaskController()
        {
            _taskService = new TaskService();
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet, Route("QuertList")]
        public HttpResponseMessage QuertList(string name = "")
        {
            var pager = new ListPager<Task>();
            if (!string.IsNullOrEmpty(name))
            {
                pager.AddFilter(m => m.Title.Contains(name));
            }
            pager = _taskService.QueryPager(pager);
            var result = new
            {
                total = pager.Total,
                rows = pager.Rows.Select(m => new
                {
                    CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    m.Id,
                    StartTime = m.StartDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    LastRunTime = m.LastRunTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    NextRunTime = m.NextRunTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    RunMode = m.RunMoreTimes ? "周期运行" : "一次运行",
                    m.Remark,
                    m.Status,
                    m.Title,
                    m.TotalRunCount
                })
            };
            return ApiResponse(result);
        }

        /// <summary>
        /// 查询详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("QuertTaskDetail")]
        public HttpResponseMessage QuertTaskDetail(Guid id)
        {
            return ApiResponse(ResultStatus.Success, "请求数据成功", _taskService.QueryById(id));
        }

        /// <summary>
        /// 查询日志记录
        /// </summary>
        /// <param name="endtime"></param>
        /// <param name="task"></param>
        /// <param name="starttime"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet, Route("QueryLogPager")]
        public HttpResponseMessage QueryLogPager(Guid? task, int? category, DateTime? starttime, DateTime? endtime)
        {
            var pager = new ListPager<SystemLog>();
            if (task.HasValue)
            {
                pager.AddFilter(m => m.TaskId == task);
            }
            if (category > 0)
            {
                pager.AddFilter(m => m.Category == category);
            }
            if (starttime.HasValue)
            {
                pager.AddFilter(m => m.CreateTime >= starttime);
            }
            if (endtime.HasValue)
            {
                pager.AddFilter(m => m.CreateTime <= endtime);
            }
            pager = _taskService.QueryLogPager(pager);
            var result = new
            {
                total = pager.Total,
                rows = pager.Rows.Select(m => new
                {
                    m.Category,
                    m.Node,
                    m.Contents,
                    CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    m.TaskId
                })
            };
            return ApiResponse(result);
        }

        /// <summary>
        /// 创建任务并启动
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost, Route("CreateTask")]
        [ApiParamValidation]
        public HttpResponseMessage CreateTask([FromBody]TaskInfo task)
        {
            var model = new Task
            {
                Id = task.Id,
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                CreateTime = DateTime.Now,
                CronExpression = task.CronExpression,
                EndDate = task.EndDate,
                CustomParamsJson = task.CustomParamsJson,
                RunMoreTimes = task.RunMoreTimes,
                Remark = task.Remark,
                StartDate = task.StartDate,
                Title = task.Title,
                Status = (int)TaskStatus.Stop,
                TotalRunCount = 0
            };
            var result = _taskService.AddTask(model);
            if (result.Status == ResultStatus.Success)
            {
                if (task.RunNow)
                {
                    var start = _taskService.TaskStart(model);
                    return ApiResponse(ResultStatus.Success, "任务创建成功！启动状态为：" + (start.Status == ResultStatus.Success ? "成功" : "失败"), model.Id);
                }
                return ApiResponse(ResultStatus.Success, "任务创建成功！", model.Id);
            }
            return ApiResponse(result);
        }

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost, Route("EditTask")]
        [ApiParamValidation]
        public HttpResponseMessage EditTask([FromBody]TaskInfo task)
        {
            var result = _taskService.EditTask(task);
            return ApiResponse(result);
        }

        /// <summary>
        /// 更新任务的开始执行时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpPost, Route("ChangeTaskStartTime")]
        public HttpResponseMessage ChangeTaskStartTime(Guid id, DateTime time)
        {
            _taskService.StopTask(id);
            var task = _taskService.QueryById(id);
            task.StartDate = time;
            _taskService.EditTask(new TaskInfo
            {
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                CronExpression = task.CronExpression,
                CustomParamsJson = task.CustomParamsJson,
                EndDate = task.EndDate,
                Remark = task.Remark,
                StartDate = task.StartDate,
                Id = task.Id,
                RunMoreTimes = task.RunMoreTimes,
                Title = task.Title
            });
            var result = _taskService.TaskStart(task);
            return ApiResponse(result);
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("StartTask")]
        public HttpResponseMessage StartTask(Guid id)
        {
            var task = _taskService.QueryById(id);
            if (task == null || task.Status != (int)TaskStatus.Stop)
            {
                return ApiResponse(ResultStatus.Failed, "任务在停止状态下才能启动！");
            }
            var result = _taskService.TaskStart(task);
            return ApiResponse(result);
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("RunOnceTask")]
        public HttpResponseMessage RunOnceTask(Guid id)
        {
            var result = _taskService.RunOnceTask(id);
            return ApiResponse(result);
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("PauseTask")]
        public HttpResponseMessage PauseTask(Guid id)
        {
            var result = _taskService.PauseTask(id);
            return ApiResponse(result);
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("ResumeTask")]
        public HttpResponseMessage ResumeTask(Guid id)
        {
            var result = _taskService.ResumeTask(id);
            return ApiResponse(result);
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("StopTask")]
        public HttpResponseMessage StopTask(Guid id)
        {
            var result = _taskService.StopTask(id);
            return ApiResponse(result);
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteTask")]
        public HttpResponseMessage DeleteTask(Guid id)
        {
            var result = _taskService.DeleteTask(id);
            return ApiResponse(result);
        }
    }
}
