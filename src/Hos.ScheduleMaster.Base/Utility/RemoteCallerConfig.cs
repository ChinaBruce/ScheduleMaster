using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Base.Utility
{
    public class RemoteCallerConfig
    {
        /// <summary>
        /// 上线前要把这个修改为正式地址
        /// </summary>
        public static string ApiHost => ConfigurationManager.AppSettings.Get("ApiHost");

        /// <summary>
        /// 这里的token用来验证请求者的身份，与api约定好即可
        /// </summary>
        public static Dictionary<string, string> Header
        {
            get
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                header.Add("token", "6a204bd89f3c8348afd5c77c717a097a");
                return header;
            }
        }
    }
}
