using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Core.Models
{
    public class TaskLock
    {
        [Key]
        public Guid TaskId { get; set; }

        [Required]
        public int Status { get; set; }
    }
}
