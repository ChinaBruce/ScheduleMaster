using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Homan.ScheduleMaster.Web.Extension
{
    /// <summary>
    /// 用json.net重写一个ControllerResult
    /// </summary>
    public class JsonNetResult : ActionResult
    {
        // 构造函数
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                //忽略掉循环引用         
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) && string.Equals(context.HttpContext.Request.HttpMethod, "GET"))
            {
                throw new InvalidOperationException("httpmethod refused");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                response.Write(JsonConvert.SerializeObject(Data, Settings));
            }
        }

        // Properties
        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public JsonSerializerSettings Settings { get; private set; }

    }

    /// <summary>
    /// 基于JsonNetResult直接在controller中返回json的扩展方法
    /// </summary>
    public static class JsonNetResultExtension
    {
        public static JsonNetResult JsonNet(this Controller controller, bool success, string msg = "", string url = "", object data = null)
        {
            return JsonNet(new { Success = success, Msg = msg, Url = url, Data = data }, null, null, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data)
        {
            return JsonNet(data, null, null, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, string contentType)
        {
            return JsonNet(data, contentType, null, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, JsonRequestBehavior behavior)
        {
            return JsonNet(data, null, null, behavior);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, string contentType, JsonRequestBehavior behavior)
        {
            return JsonNet(data, contentType, null, behavior);
        }

        /// <summary>
        /// 创建JsonNetResult对象，输出json数据到客户端
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="encoding"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        private static JsonNetResult JsonNet(object data, string contentType, Encoding encoding,
    JsonRequestBehavior behavior)
        {
            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = encoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}