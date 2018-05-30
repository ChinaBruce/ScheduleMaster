using Homan.ScheduleMaster.Core.Common;
using Homan.ScheduleMaster.Core.EntityFramework;
using Homan.ScheduleMaster.Core.Log;
using Homan.ScheduleMaster.Core.Dto;
using Homan.ScheduleMaster.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Homan.ScheduleMaster.Base.Utility;

namespace Homan.ScheduleMaster.Core.Service
{
    public class TaskService : BaseService, ITaskService
    {
        public List<Task> QueryAll()
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                return db.Task.ToList();
            }
        }

        public ListPager<Task> QueryPager(ListPager<Task> pager)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                return _repositoryFactory.Tasks.WherePager(pager, m => m.Status != (int)TaskStatus.Deleted, m => m.CreateTime, false);
            }
        }

        public ListPager<SystemLog> QueryLogPager(ListPager<SystemLog> pager)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                return _repositoryFactory.SystemLogs.WherePager(pager, m => true, m => m.Id, false);
            }
        }

        public int DeleteLog(Guid? task, int? category, DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<SystemLog, bool>> where = m => true;
            if (task.HasValue)
            {
                where = where.And(x => x.TaskId == task.Value);
            }
            if (category.HasValue)
            {
                where = where.And(x => x.Category == category.Value);
            }
            if (startdate.HasValue)
            {
                where = where.And(x => x.CreateTime >= startdate.Value);
            }
            if (enddate.HasValue)
            {
                where = where.And(x => x.CreateTime < enddate.Value);
            }
            _repositoryFactory.SystemLogs.DeleteBy(where);
            return UnitOfWork.Commit();
        }

        public Task QueryById(Guid id)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                return db.Task.FirstOrDefault(m => m.Id == id);
            }
        }

        public ApiResponseMessage AddTask(Task model)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                model.CreateTime = DateTime.Now;
                db.Task.Add(model);
                if (db.SaveChanges() > 0)
                {
                    //创建专属目录
                    //string path = $"{AppDomain.CurrentDomain.BaseDirectory}/TaskAssembly/{model.AssemblyName}";
                    //if (!System.IO.Directory.Exists(path))
                    //{
                    //    System.IO.Directory.CreateDirectory(path);
                    //}
                    return ServiceResult(ResultStatus.Success, "任务创建成功!");
                }
                return ServiceResult(ResultStatus.Failed, "数据保存失败!");
            }
        }

        public ApiResponseMessage EditTask(TaskInfo model)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var task = db.Task.FirstOrDefault(m => m.Id == model.Id);
                if (task == null)
                {
                    return ServiceResult(ResultStatus.Failed, "任务不存在!");
                }
                if (task.Status != (int)TaskStatus.Stop)
                {
                    return ServiceResult(ResultStatus.Failed, "在停止状态下才能编辑任务信息!");
                }
                var update = UpdateTask(m => m.Id == task.Id, m => new Task
                {
                    AssemblyName = model.AssemblyName,
                    ClassName = model.ClassName,
                    CronExpression = model.CronExpression,
                    EndDate = model.EndDate,
                    Remark = model.Remark,
                    CustomParamsJson = model.CustomParamsJson,
                    RunMoreTimes = model.RunMoreTimes,
                    StartDate = model.StartDate,
                    Title = model.Title
                });
                if (update)
                {
                    return ServiceResult(ResultStatus.Success, "任务编辑成功!");
                }
                return ServiceResult(ResultStatus.Failed, "任务编辑失败!");
            }
        }

        public ApiResponseMessage TaskStart(Task task)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                bool success = false;
                //启动任务
                var list = db.ServerNodes.Where(m => m.Status == 1).ToList();
                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    {"Id", task.Id.ToString()},
                    {"AssemblyName", task.AssemblyName},
                    {"ClassName", task.ClassName},
                    {"RunMoreTimes", task.RunMoreTimes.ToString()},
                    {"CustomParamsJson", task.CustomParamsJson},
                    {"Title", task.Title},
                    {"CronExpression", task.CronExpression},
                    {"StartDate", task.StartDate.ToString()},
                    {"EndDate", task.EndDate.ToString()}
                };
                var header = Header;
                foreach (var node in list)
                {
                    var result = HttpRequest.Send($"http://{node.Host}/api/Quartz/Start", "post", param, header);
                    success = success || result.Key == System.Net.HttpStatusCode.OK;
                }
                if (success)
                {
                    //启动成功后更新任务状态为运行中
                    _repositoryFactory.Tasks.ModifyBy(m => m.Id == task.Id, m => new Task
                    {
                        Status = (int)TaskStatus.Running
                    });
                    if (db.SaveChanges() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务启动成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    _repositoryFactory.Tasks.ModifyBy(m => m.Id == task.Id, m => new Task
                    {
                        Status = (int)TaskStatus.Stop,
                        NextRunTime = null
                    });
                    db.SaveChanges();
                    return ServiceResult(ResultStatus.Failed, "任务启动失败!");
                }
            }

        }

        public ApiResponseMessage RunOnceTask(Guid id)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var task = db.Task.FirstOrDefault(m => m.Id == id);
                if (task != null && task.Status == (int)TaskStatus.Running)
                {
                    //根据节点权重来选择一个节点运行
                    var list = db.ServerNodes.Where(m => m.Status == 1).OrderBy(x => x.Priority).ToList();
                    int[] arry = new int[list.Count + 1];
                    arry[0] = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        arry[i + 1] = list[i].Priority + arry[i];
                    }
                    var sum = list.Sum(x => x.Priority);
                    int rnd = new Random().Next(0, sum);
                    string host = string.Empty;
                    for (int i = 1; i < arry.Length; i++)
                    {
                        if (rnd >= arry[i - 1] && rnd < arry[i])
                        {
                            host = list[i - 1].Host;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(host))
                    {
                        var result = HttpRequest.Send($"http://{host}/api/Quartz/RunOnce/{task.Id.ToString()}", "post", null, Header);
                        if (result.Key == System.Net.HttpStatusCode.OK)
                        {
                            //运行成功后更新信息
                            task.LastRunTime = DateTime.Now;
                            task.TotalRunCount += 1;
                            db.SaveChanges();
                            return ServiceResult(ResultStatus.Success, "任务运行成功!");
                        }
                        else
                        {
                            LogHelper.Error($"任务运行失败！{result.Key}：{result.Value}");
                            return ServiceResult(ResultStatus.Failed, "任务运行失败!");
                        }
                    }
                    else
                    {
                        return ServiceResult(ResultStatus.Failed, "没有可用的任务节点!");
                    }
                }
            }
            return ServiceResult(ResultStatus.Failed, "任务不在运行状态下!");
        }

        public ApiResponseMessage PauseTask(Guid id)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var task = db.Task.FirstOrDefault(m => m.Id == id);
                if (task != null && task.Status == (int)TaskStatus.Running)
                {
                    bool success = false;
                    var list = db.ServerNodes.Where(m => m.Status == 1).ToList();
                    var header = Header;
                    foreach (var node in list)
                    {
                        var result = HttpRequest.Send($"http://{node.Host}/api/Quartz/Pause/{task.Id.ToString()}", "post", null, header);
                        success = success || result.Key == System.Net.HttpStatusCode.OK;
                    }
                    if (success)
                    {
                        //暂停成功后更新任务状态为已暂停
                        _repositoryFactory.Tasks.ModifyBy(m => m.Id == task.Id, m => new Task
                        {
                            Status = (int)TaskStatus.Paused,
                            NextRunTime = null
                        });
                        if (db.SaveChanges() > 0)
                        {
                            return ServiceResult(ResultStatus.Success, "任务暂停成功!");
                        }
                        return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                    }
                    else
                    {
                        return ServiceResult(ResultStatus.Failed, "任务暂停失败!");
                    }
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能暂停!");
        }

        public ApiResponseMessage ResumeTask(Guid id)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var task = db.Task.FirstOrDefault(m => m.Id == id);
                if (task != null && task.Status == (int)TaskStatus.Paused)
                {
                    bool success = false;
                    var list = db.ServerNodes.Where(m => m.Status == 1).ToList();
                    var header = Header;
                    foreach (var node in list)
                    {
                        var result = HttpRequest.Send($"http://{node.Host}/api/Quartz/Resume/{task.Id.ToString()}", "post", null, header);
                        success = success || result.Key == System.Net.HttpStatusCode.OK;
                    }
                    if (success)
                    {
                        //恢复运行后更新任务状态为运行中
                        _repositoryFactory.Tasks.ModifyBy(m => m.Id == task.Id, m => new Task
                        {
                            Status = (int)TaskStatus.Running
                        });
                        if (db.SaveChanges() > 0)
                        {
                            return ServiceResult(ResultStatus.Success, "任务恢复成功!");
                        }
                        return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                    }
                    else
                    {
                        return ServiceResult(ResultStatus.Failed, "任务恢复失败!");
                    }
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能恢复运行!");
        }

        public ApiResponseMessage StopTask(Guid id)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var task = db.Task.FirstOrDefault(m => m.Id == id);
                if (task != null && task.Status > (int)TaskStatus.Stop)
                {
                    bool success = false;
                    var list = db.ServerNodes.Where(m => m.Status == 1).ToList();
                    var header = Header;
                    foreach (var node in list)
                    {
                        var result = HttpRequest.Send($"http://{node.Host}/api/Quartz/Stop/{task.Id.ToString()}", "post", null, header);
                        success = success || result.Key == System.Net.HttpStatusCode.OK;
                    }
                    if (success)
                    {
                        //更新任务状态为已停止
                        _repositoryFactory.Tasks.ModifyBy(m => m.Id == task.Id, m => new Task
                        {
                            Status = (int)TaskStatus.Stop,
                            NextRunTime = null
                        });
                        if (db.SaveChanges() > 0)
                        {
                            return ServiceResult(ResultStatus.Success, "任务已停止运行!");
                        }
                        return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                    }
                    else
                    {
                        return ServiceResult(ResultStatus.Failed, "任务停止失败!");
                    }
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能停止!");
        }

        public ApiResponseMessage DeleteTask(Guid id)
        {
            var task = QueryById(id);
            if (task != null && task.Status != (int)TaskStatus.Deleted)
            {
                if (task.Status == (int)TaskStatus.Stop)
                {
                    //停止状态下的才能删除
                    var update = UpdateTask(m => m.Id == task.Id, m => new Task
                    {
                        Status = (int)TaskStatus.Deleted,
                        NextRunTime = null
                    });
                    if (update)
                    {
                        return ServiceResult(ResultStatus.Success, "任务已删除!");
                    }
                    return ServiceResult(ResultStatus.Failed, "任务删除失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能删除!");
        }

        public bool UpdateTask(Expression<Func<Task, bool>> where, Expression<Func<Task, object>> updater)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                _repositoryFactory.Tasks.ModifyBy(where, updater);
                return db.SaveChanges() > 0;
            }
        }
    }
}