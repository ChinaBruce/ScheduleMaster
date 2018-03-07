using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Core.Models
{
    public class OptionConfig
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Key { get; set; }

        [Required, MaxLength(1000)]
        public string Value { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
