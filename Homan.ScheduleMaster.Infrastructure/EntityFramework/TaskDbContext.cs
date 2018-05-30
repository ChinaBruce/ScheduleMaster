using Homan.ScheduleMaster.Base.Models;
using Homan.ScheduleMaster.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Homan.ScheduleMaster.Core.EntityFramework
{
    public class TaskDbContext : DbContext, IUnitOfWork
    {
        public TaskDbContext() : base("DefaultConn")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public int Commit()
        {
            return base.SaveChanges();
        }

        public async System.Threading.Tasks.Task<int> CommitAsync()
        {
            return await base.SaveChangesAsync();
        }

        public virtual DbSet<Task> Task { get; set; }

        public virtual DbSet<SystemLog> SystemLog { get; set; }

        public virtual DbSet<ServerNodes> ServerNodes { get; set; }

        public virtual DbSet<OptionConfig> OptionConfig { get; set; }

        public virtual DbSet<TaskLock> TaskLock { get; set; }
    }
}
