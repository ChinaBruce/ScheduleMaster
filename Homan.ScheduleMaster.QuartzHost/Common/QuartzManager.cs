
using Homan.ScheduleMaster.Core;
using Homan.ScheduleMaster.Core.Models;
using Homan.ScheduleMaster.Core.Utility;
using Homan.ScheduleMaster.Infrastructure.EntityFramework;
using Homan.ScheduleMaster.Infrastructure.Log;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Homan.ScheduleMaster.QuartzHost.Common
{
    public class QuartzManager
    {
        private QuartzManager()
        {
        }

        private static readonly object obj = new object();

        private static IScheduler _scheduler = null;

        /// <summary>
        /// 初始化调度系统
        /// </summary>
        public static void InitScheduler()
        {
            try
            {
                lock (obj)
                {
                    if (_scheduler == null)
                    {
                        NameValueCollection properties = new NameValueCollection();

                        properties["quartz.scheduler.instanceName"] = "ExampleDefaultQuartzScheduler";

                        properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";

                        properties["quartz.threadPool.threadCount"] = "50";

                        properties["quartz.threadPool.threadPriority"] = "Normal";

                        properties["quartz.jobStore.misfireThreshold"] = "60000";

                        properties["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";

                        ISchedulerFactory factory = new StdSchedulerFactory(properties);

                        _scheduler = factory.GetScheduler();
                        _scheduler.Clear();
                        _scheduler.Start();
                        CreateActiveAppJob();
                        SQLHelper.ExecuteNonQuery($"UPDATE ServerNodes SET Status=1 WHERE NodeName='{ConfigurationManager.AppSettings.Get("HostIdentity")}' ");
                        LogHelper.Info("任务调度平台初始化成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("任务调度平台初始化失败！", ex);
            }
        }

        /// <summary>
        /// 关闭调度系统
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                //判断调度是否已经关闭
                if (!_scheduler.IsShutdown)
                {
                    //等待任务运行完成再关闭调度
                    _scheduler.Shutdown(true);
                    LogHelper.Info("任务调度平台已经停止！");
                    SQLHelper.ExecuteNonQuery($"UPDATE ServerNodes SET Status=0 WHERE NodeName='{ConfigurationManager.AppSettings.Get("HostIdentity")}' ");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("任务调度平台停止失败！", ex);
            }
        }

        /// <summary>
        /// 启动一个任务，带重试机制
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool StartWithRetry(Task task)
        {
            AppDomain ad = null;
            try
            {
                //这里用AppDomain解决程序集引用依赖的问题
                ad = AssemblyHelper.LoadAppDomain(task.AssemblyName);
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        Start(task, ad);
                        return true;
                    }
                    catch (SchedulerException sexp)
                    {
                        LogHelper.Error($"任务启动失败！开始第{i + 1}次重试...", sexp, task.Id);
                    }
                }
                //最后一次尝试
                Start(task, ad);
                return true;
            }
            catch (SchedulerException sexp)
            {
                AssemblyHelper.UnLoadAppDomain(ad);
                LogHelper.Error($"任务所有重试都失败了，已放弃启动！", sexp, task.Id);
                return false;
            }
            catch (Exception exp)
            {
                AssemblyHelper.UnLoadAppDomain(ad);
                LogHelper.Error($"任务启动失败！", exp, task.Id);
                return false;
            }
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static bool Pause(Guid taskId)
        {
            try
            {
                JobKey jk = new JobKey(taskId.ToString().ToLower());
                if (_scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    _scheduler.PauseJob(jk);
                    var jobDetail = _scheduler.GetJobDetail(jk);
                    if (jobDetail.JobType.GetInterface("IInterruptableJob") != null)
                    {
                        _scheduler.Interrupt(jk);
                    }
                    LogHelper.Warn($"任务已经暂停运行！", taskId);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"任务暂停运行失败！", exp, taskId);
                return false;
            }
        }

        /// <summary>
        /// 恢复运行
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static bool Resume(Guid taskId)
        {
            try
            {
                JobKey jk = new JobKey(taskId.ToString().ToLower());
                if (_scheduler.CheckExists(jk))
                {
                    //恢复任务继续执行
                    _scheduler.ResumeJob(jk);
                    LogHelper.Info($"任务已经恢复运行！", taskId);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"任务恢复运行失败！", exp, taskId);
                return false;
            }
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool Stop(Guid taskId)
        {
            try
            {
                JobKey jk = new JobKey(taskId.ToString().ToLower());
                var job = _scheduler.GetJobDetail(jk);
                if (job != null)
                {
                    var instance = job.JobDataMap["instance"] as TaskBase;
                    //释放资源
                    if (instance != null)
                    {
                        instance.Dispose();
                    }
                    //卸载应用程序域
                    var domain = job.JobDataMap["domain"] as AppDomain;
                    AssemblyHelper.UnLoadAppDomain(domain);
                    //删除quartz有关设置
                    var trigger = new TriggerKey(taskId.ToString());
                    _scheduler.PauseTrigger(trigger);
                    _scheduler.UnscheduleJob(trigger);
                    _scheduler.DeleteJob(jk);
                    _scheduler.ListenerManager.RemoveJobListener(taskId.ToString());
                }
                LogHelper.Info($"任务已经停止运行！", taskId);
                return true;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"任务停止失败！", exp, taskId);
                return false;
            }

        }

        /// <summary>
        ///立即运行一次任务
        /// </summary>
        /// <param name="JobKey">任务key</param>
        public static bool RunOnce(Guid taskId)
        {
            JobKey jk = new JobKey(taskId.ToString().ToLower());
            if (_scheduler.CheckExists(jk))
            {
                var jobDetail = _scheduler.GetJobDetail(jk);
                var instance = jobDetail.JobDataMap["instance"] as TaskBase;
                try
                {
                    if (instance != null)
                    {
                        instance.TaskId = taskId;

                        var param = jobDetail.JobDataMap["params"];
                        if (param != null)
                        {
                            instance.CustomParamsJson = param.ToString();
                        }
                        instance.InnerRun();
                        LogHelper.Info(string.Format("任务[{0}]立即运行成功！", jobDetail.JobDataMap["name"]), taskId);
                        return true;
                    }
                    else
                    {
                        LogHelper.Error($"instance=null", taskId);
                    }
                }
                catch (Exception exp)
                {
                    LogHelper.Error($"任务[{jobDetail.JobDataMap["name"]}]运行失败！", exp, taskId);
                }
                //var triggers = _scheduler.GetTriggersOfJob(jk);
                //string taskName = JobKey;
                //if (triggers != null && triggers.Count > 0)
                //{
                //    taskName = triggers[0].Description;
                //}
                //var type = jobDetail.JobType;
                //var instance = type.FastNew();
                //var method = type.GetMethod("Execute");
                //method.Invoke(instance, new object[] { null });
            }
            else
            {
                LogHelper.Error($"_scheduler.CheckExists=false", taskId);
            }
            return false;
        }

        #region 私有方法

        /// <summary>
        /// 创建激活系统的周期任务，避免应用程序池被回收
        /// </summary>
        private static void CreateActiveAppJob()
        {
            IJobDetail job = JobBuilder.Create(typeof(ActiveAppJob)).WithIdentity("active_app_job").Build();
            //每隔10分钟运行一次
            ITrigger trigger = TriggerBuilder.Create().WithIdentity("active_app_trigger").StartNow().WithCronSchedule("0 0/10 * * * ? *").Build();
            _scheduler.ScheduleJob(job, trigger);
            //LogHelper.Info($"[系统激活任务]启动成功！");
        }

        private static void Start(Task task, AppDomain domain)
        {
            //throw new SchedulerException("SchedulerException");

            //在应用程序域中创建实例返回并保存在job中，这是最终调用任务执行的实例
            TaskBase instance = domain.CreateInstanceFromAndUnwrap(AssemblyHelper.GetTaskAssemblyPath(task.AssemblyName), task.ClassName) as TaskBase;
            if (instance == null)
            {
                throw new InvalidCastException($"任务实例创建失败，请确认目标任务是否派生自TaskBase类型。程序集：{task.AssemblyName}，类型：{task.ClassName}");
            }
            // instance.logger = new LogWriter(); ;
            JobDataMap map = new JobDataMap
            {
                new KeyValuePair<string, object> ("domain",domain),
                new KeyValuePair<string, object> ("instance",instance),
                new KeyValuePair<string, object> ("name",task.Title),
                 new KeyValuePair<string, object> ("params",task.CustomParamsJson)
            };
            string jobName = task.Id.ToString().ToLower();
            IJobDetail job = JobBuilder.Create(typeof(RootJob)).WithIdentity(jobName)
                .SetJobData(map)
                //.UsingJobData("assembly", task.AssemblyName)
                //.UsingJobData("class", task.ClassName)
                .Build();

            //添加触发器
            _scheduler.ListenerManager.AddJobListener(new JobRunListener(jobName),
                KeyMatcher<JobKey>.KeyEquals(new JobKey(jobName)));

            if (task.RunMoreTimes)
            {
                if (!CronExpression.IsValidExpression(task.CronExpression))
                {
                    throw new Exception("cron表达式验证失败");
                }
                CronTriggerImpl trigger = new CronTriggerImpl
                {
                    CronExpressionString = task.CronExpression,
                    Name = task.Title,
                    Key = new TriggerKey(task.Id.ToString()),
                    Description = task.Remark
                };
                if (task.StartDate.HasValue)
                {
                    trigger.StartTimeUtc = TimeZoneInfo.ConvertTimeToUtc(task.StartDate.Value);
                }
                if (task.EndDate.HasValue)
                {
                    trigger.EndTimeUtc = TimeZoneInfo.ConvertTimeToUtc(task.EndDate.Value);
                }

                _scheduler.ScheduleJob(job, trigger);
            }
            else
            {
                DateTimeOffset start = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
                if (task.StartDate.HasValue)
                {
                    start = TimeZoneInfo.ConvertTimeToUtc(task.StartDate.Value);
                }
                DateTimeOffset end = start.AddMinutes(1);
                if (task.EndDate.HasValue)
                {
                    end = TimeZoneInfo.ConvertTimeToUtc(task.EndDate.Value);
                }
                ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity(jobName)
               .StartAt(start)
               .WithSimpleSchedule(x => x
                  .WithRepeatCount(1).WithIntervalInMinutes(1))
                  .EndAt(end)
               .Build();
                _scheduler.ScheduleJob(job, trigger);
            }

            LogHelper.Info($"任务\"{task.Title}\"启动成功！", task.Id);

            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    var log = instance.ReadLog();
                    if (log != null)
                    {
                        //System.Diagnostics.Debug.WriteLine("queue：" + log.Contents);
                        LogManager.Queue.Write(log);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(3000);
                    }
                }
            });
        }
        #endregion
    }

    /// <summary>
    /// 任务运行状态监听器
    /// </summary>
    internal class JobRunListener : IJobListener
    {
        public string Name { get; set; }

        public JobRunListener()
        {
        }

        public JobRunListener(string name)
        {
            Name = name;
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {

        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {

        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            var taskId = Guid.Empty;
            if (Guid.TryParse(Name, out taskId))
            {
                using (TaskDbContext db = new TaskDbContext())
                {
                    var task = db.Task.FirstOrDefault(m => m.Id == taskId && m.Status > (int)TaskStatus.Deleted);
                    if (task == null)
                    {
                        return;
                    }
                    var stop = false;
                    if (jobException == null)
                    {
                        var utcDate = context.Trigger.GetNextFireTimeUtc();
                        var nextRunTime = utcDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value.DateTime, TimeZoneInfo.Local) : new DateTime?();

                        task.LastRunTime = DateTime.Now;
                        task.TotalRunCount = task.TotalRunCount + 1;
                        if (task.RunMoreTimes)
                        {
                            task.NextRunTime = nextRunTime;
                            task.Status = (int)TaskStatus.Running;
                        }
                        else
                        {
                            stop = true;
                        }
                    }
                    else
                    {
                        //对只执行 一次并且本次抢锁失败的任务，在这里停止掉
                        if (jobException.Message == "lockfailed" && !task.RunMoreTimes)
                        {
                            stop = true;
                        }
                    }
                    if (stop)
                    {
                        QuartzManager.Stop(taskId);
                        task.Status = (int)TaskStatus.Stop;
                        task.NextRunTime = null;
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}