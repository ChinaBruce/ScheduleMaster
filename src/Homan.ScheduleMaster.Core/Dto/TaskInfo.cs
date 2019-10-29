using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Homan.ScheduleMaster.Core.Dto
{
    public class TaskInfo
    {
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        public bool RunMoreTimes { get; set; }

        [MaxLength(50)]
        public string CronExpression { get; set; }

        [Required, MaxLength(200)]
        public string AssemblyName { get; set; }

        [Required, MaxLength(200)]
        public string ClassName { get; set; }

        [MaxLength(500)]
        public string CustomParamsJson { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool RunNow { get; set; }

        public TaskInfo()
        {
            this.Id = Guid.NewGuid();
        }
    }
}