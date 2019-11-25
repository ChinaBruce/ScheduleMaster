using Homan.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Core.Log
{
    public class LogHelper
    {

        public static void Info(string message)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Info,
                Message = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Info(string message, int task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Info,
                Message = message,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Warn(string message)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Warn,
                Message = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Warn(string message, int task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Warn,
                Message = message,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(Exception ex)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(Exception ex, int task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Exception exp)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = $"{message}，ERROR：{exp.Message}",
                StackTrace = exp.StackTrace,
                TaskId = 0,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Exception exp, int task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = $"{message}，ERROR：{exp.Message}",
                StackTrace = exp.StackTrace,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }
    }

}
