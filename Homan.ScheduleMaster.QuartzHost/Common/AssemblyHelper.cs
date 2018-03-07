using Homan.ScheduleMaster.Core;
using Homan.ScheduleMaster.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Homan.ScheduleMaster.QuartzHost.Common
{
    public class AssemblyHelper
    {
        public static Type GetClassType(string assemblyPath, string className)
        {
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
                Type type = assembly.GetType(className, true, true);
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T CreateInstance<T>(Type type) where T : class
        {
            try
            {
                return Activator.CreateInstance(type) as T;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static TaskBase CreateTaskInstance(string assemblyName, string className)
        {
            try
            {
                Type type = GetClassType(GetTaskAssemblyPath(assemblyName), className);
                return CreateInstance<TaskBase>(type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetTaskAssemblyPath(string assemblyName)
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}\\TaskAssembly\\{assemblyName}\\{assemblyName}.dll";
            //return $"{AppDomain.CurrentDomain.BaseDirectory}\\TaskAssembly\\{assemblyName}.dll";
        }

        /// <summary>
        /// 加载应用程序域
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static AppDomain LoadAppDomain(string assemblyName)
        {
            try
            {
                string dllPath = GetTaskAssemblyPath(assemblyName);
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationName = assemblyName;
                setup.ApplicationBase = Path.GetDirectoryName(dllPath);
                if (File.Exists(dllPath + ".config"))
                {
                    setup.ConfigurationFile = dllPath + ".config";
                }
                //setup.ShadowCopyFiles = "true"; //启用影像复制程序集
                //setup.ShadowCopyDirectories = setup.ApplicationBase;
                //AppDomain.CurrentDomain.SetShadowCopyFiles();
                AppDomain domain = AppDomain.CreateDomain(assemblyName, null, setup);
                //AppDomain.MonitoringIsEnabled = true;
                return domain;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"加载应用程序域{assemblyName}失败！", exp);
                throw exp;
            }
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        /// <param name="domain"></param>
        public static void UnLoadAppDomain(AppDomain domain)
        {
            if (domain != null)
            {
                AppDomain.Unload(domain);
                //卸载程序域后CLR会强制进行垃圾回收，该过程在调用线程中是同步进行的
            }
        }
    }
}