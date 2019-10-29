using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Base.Models
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        [Required]
        public bool RunMoreTimes { get; set; }

        [MaxLength(50)]
        public string CronExpression { get; set; }

        [Required, MaxLength(200)]
        public string AssemblyName { get; set; }

        [Required, MaxLength(200)]
        public string ClassName { get; set; }

        [MaxLength(500)]
        public string CustomParamsJson { get; set; }

        [Required]
        public int Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? LastRunTime { get; set; }

        public DateTime? NextRunTime { get; set; }

        public int TotalRunCount { get; set; }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = -1,

        /// <summary>
        /// 已停止
        /// </summary>
        Stop = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused = 2

    }
}
