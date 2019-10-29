using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Base.Models
{
    [Serializable]
    public class SystemLog
    {
        public SystemLog()
        {
        }

        public SystemLog(string node)
        {
            //this.TaskId = Guid.Empty;//表示系统运行日志
            this.Node = node;// ConfigurationManager.AppSettings.Get("HostIdentity");
        }

        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Category { get; set; }

        [Required]
        public string Contents { get; set; }

        public Guid? TaskId { get; set; }

        public string Node { get; set; }

        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogCategory
    {
        /// <summary>
        /// 普通信息
        /// </summary>
        Info = 1,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 2,

        /// <summary>
        /// 异常
        /// </summary>
        Error = 3
    }
}
