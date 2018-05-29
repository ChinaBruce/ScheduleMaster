using Homan.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Infrastructure.Log
{
    public class LogHelper
    {

        public static void Info(string message)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Info,
                Contents = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Info(string message, Guid task)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Info,
                Contents = message,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Warn(string message)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Warn,
                Contents = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Warn(string message, Guid task)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Warn,
                Contents = message,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Error,
                Contents = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Guid task)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Error,
                Contents = message,
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Exception exp)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Error,
                Contents = $"##{message}##  {exp.Message}",
                TaskId = Guid.Empty,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Exception exp, Guid task)
        {
            LogManager.Queue.Write(new SystemLog
            {
                Category = (int)LogCategory.Error,
                Contents = $"##{message}##  {exp.Message}",
                TaskId = task,
                CreateTime = DateTime.Now
            });
        }
    }

}
