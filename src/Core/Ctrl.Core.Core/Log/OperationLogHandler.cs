using Ctrl.Core.Core.Converts;
using Ctrl.Core.Core.Http;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace Ctrl.Core.Core.Log
{
    public class OperationLogHandler:BaseHandler<OperateLog>
    {
        /// <summary>
        ///     操作日志
        /// </summary>
        public OperationLogHandler(HttpRequest httpRequest) : base("OperateLogToDatabase") {
            var request = HttpContexts.Current.Request;
            log = new OperateLog()
            {
                OperateLogId = CombUtil.NewComb(),
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ServerHost = String.Format("{0}【{1}】", IpBrowserUtil.GetServerHost(), IpBrowserUtil.GetServerHostIp()),
                ClientHost = String.Format("{0}", IpBrowserUtil.GetClientIp()),
                RequestContentLength = httpRequest.ContentLength == null ? 0 : (int)httpRequest.ContentLength,
                RequestType = request.Method,
                UserAgent = request.Headers[HeaderNames.UserAgent]
            };

            if (request?.HasFormContentType ?? request.HasFormContentType)
            {
                log.RequestData = request?.Form?.ToJson();
            }
            else
            {
                if (request.Body.CanSeek)
                {
                    log.RequestData = HttpUtility.UrlDecode(new StreamReader(request?.Body).ReadToEnd());
                }
                //if (request?.Body != null)
                //{
                //    log.RequestData = HttpUtility.UrlDecode(new StreamReader(request?.Body).ReadToEnd());
                //}
                //else if (request?.Form != null)
                //{
                //    log.RequestData = request?.Form?.ToJson();
                //}

            }

            log.Url =new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
            log.UrlReferrer = httpRequest.Headers[HeaderNames.Referer];
        }
        /// <summary>
        /// 执行时间
        /// </summary>
        public void ActionExecuted()
        {
            log.ActionExecutionTime = (DateTime.Now - Convert.ToDateTime(log.CreateTime)).TotalSeconds;
        }

        /// <summary>
        /// 页面展示时间
        /// </summary>
        /// <param name="responseBase"></param>
        public void ResultExecuted(HttpResponse responseBase)
        {
            log.ResponseStatus = responseBase.StatusCode.ToString();
            //页面展示时间
            log.ResultExecutionTime = (DateTime.Now -Convert.ToDateTime(log.CreateTime)).TotalSeconds;
        }
    }
}
