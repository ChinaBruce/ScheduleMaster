using Homan.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Homan.ScheduleMaster.Infrastructure.EntityFramework
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext() : base("DefaultConn")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Task> Task { get; set; }

        public virtual DbSet<SystemLog> SystemLog { get; set; }

        public virtual DbSet<ServerNodes> ServerNodes { get; set; }

        public virtual DbSet<OptionConfig> OptionConfig { get; set; }

        public virtual DbSet<TaskLock> TaskLock { get; set; }
    }
}
