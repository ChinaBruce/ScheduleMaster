using Homan.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Core
{
    /// <summary>
    /// 任务抽象基类，要加入的任务必须继承此类
    /// by hoho
    /// </summary>
    public abstract class TaskBase : MarshalByRefObject, IDisposable
    {
        bool _isRunning = false;

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// 任务id，每次运行前都会重新赋值，方便写log或其他操作时跟踪
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public string CustomParamsJson { get; set; }

        /// <summary>
        /// 停止任务后可能需要的处理
        /// </summary>
        public virtual void Dispose()
        {
            //TODO:
        }

        /// <summary>
        /// 任务执行的方法，由具体任务去重写实现
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 保证前一次运行完才开始下一次，否则就跳过本次执行
        /// </summary>
        public void InnerRun()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                try
                {
                    Run();
                }
                catch (Exception err)
                {
                    throw err;
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }


        #region 日志处理

        private Queue<SystemLog> logger = new Queue<SystemLog>();

        protected void WriteLog(string message, LogCategory type = LogCategory.Info)
        {
            logger.Enqueue(new SystemLog(Node)
            {
                Category = (int)type,
                Contents = message,
                CreateTime = DateTime.Now,
                TaskId = TaskId
            });
        }

        public SystemLog ReadLog()
        {
            SystemLog q = null;
            if (logger.Count > 0)
            {
                q = logger.Dequeue();
            }
            return q;
        }

        #endregion
    }
}
