using Homan.ScheduleMaster.Core.Utility;
using Homan.ScheduleMaster.Infrastructure.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Homan.ScheduleMaster.QuartzHost.Common
{
    /// <summary>
    /// 周期性执行这个任务，可以防止应用程序池被IIS回收
    /// </summary>
    public class ActiveAppJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string url = ConfigurationManager.AppSettings.Get("ActiveUrl");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Stream stream = webResponse.GetResponseStream();
                using (stream)
                {
                    var node = ConfigurationManager.AppSettings.Get("HostIdentity");
                    SQLHelper.ExecuteNonQuery($"UPDATE ServerNodes SET LastUpdateTime='{DateTime.Now.ToString()}',Status=1 WHERE NodeName='{node}' ");
                    LogHelper.Info($"[系统激活任务]运行成功！");
                }
            }
            catch (Exception exp)
            {
                LogHelper.Error($"[系统激活任务]运行失败！", exp);
            }

        }
    }
}