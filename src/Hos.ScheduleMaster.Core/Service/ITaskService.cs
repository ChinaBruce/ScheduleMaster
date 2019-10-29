using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Base.Models;

namespace Hos.ScheduleMaster.Core.Service
{
    public interface ITaskService
    {
        List<Task> QueryAll();

        ListPager<Task> QueryPager(ListPager<Task> pager);

        ListPager<SystemLog> QueryLogPager(ListPager<SystemLog> pager);

        int DeleteLog(Guid? task, int? category, DateTime? startdate, DateTime? enddate);

        Task QueryById(Guid id);

        ApiResponseMessage AddTask(Task model);

        ApiResponseMessage EditTask(TaskInfo model);

        ApiResponseMessage TaskStart(Task task);

        ApiResponseMessage PauseTask(Guid id);

        ApiResponseMessage ResumeTask(Guid id);

        ApiResponseMessage RunOnceTask(Guid id);

        ApiResponseMessage StopTask(Guid id);

        ApiResponseMessage DeleteTask(Guid id);
    }
}
