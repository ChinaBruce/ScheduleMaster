
using Homan.ScheduleMaster.Core;
using Homan.ScheduleMaster.Core.Models;
using Homan.ScheduleMaster.Infrastructure.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Infrastructure.Log
{
    public class LogManager
    {
        public static BufferQueue<SystemLog> Queue;

        public static void Init()
        {
            Queue = new BufferQueue<SystemLog>();
            var td = new System.Threading.Thread(() =>
            {
                using (TaskDbContext db = new TaskDbContext())
                {
                    while (true)
                    {
                        Queue.Read((item, index) =>
                        {
                            db.SystemLog.Add(item);
                        });
                        db.SaveChanges();
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            });
            td.IsBackground = true;
            td.Start();
        }

        public static void Shutdown()
        {
            Queue.Clear();
        }
    }
}
