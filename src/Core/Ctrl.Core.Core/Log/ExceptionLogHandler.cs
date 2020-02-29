using Ctrl.Core.Core.Auth;
using Ctrl.Core.Core.Converts;
using Ctrl.Core.Core.Http;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Core.Web;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace Ctrl.Core.Core.Log
{
    /// <summary>
    ///     异常日志记录
    /// </summary>
    public class ExceptionLogHandler: BaseHandler<ExceptionLog>
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionLogHandler(Exception exception) : base("ExceptionLogToDatabase") {
            PrincipalUser principalUser = new PrincipalUser();
            var current = HttpContexts.Current;
            if (current != null)
            {
                principalUser = AuthenticationExtension.Current();
            }
            if (principalUser == null)
            {
                principalUser = new PrincipalUser()
                {
                    Name = "匿名用户",
                    UserId = Guid.Empty
                };
            }
            log = new ExceptionLog()
            {
                ExceptionLogId = CombUtil.NewComb().ToString(),
                CreateUserCode=principalUser.Code,
                CreateUserId=principalUser.UserId.ToString(),
                CreateUserName=principalUser.Name,
                ServerHost = String.Format("{0}【{1}】", IpBrowserUtil.GetServerHost(), IpBrowserUtil.GetServerHostIp()),
                ClientHost = String.Format("{0}", IpBrowserUtil.GetClientIp()),
                Runtime = "Web",
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                ExceptionType = exception.GetType().FullName,
                ClientAddress = IpBrowserUtil.GetAddressByApi()
                
            };
            //获取服务器信息
            var request = HttpContexts.Current.Request;
            log.RequestUrl = string.Format("{0} ", request.Path);
            log.HttpMethod = request.Method;
            log.UserAgent = request.Headers["user-agent"];
            log.InnerException = exception.InnerException != null ? GetExceptionFullMessage(exception.InnerException) : "";
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
            }
        }
        /// <summary>
        /// 获取完整的异常消息，包括内部异常消息
        /// </summary>
        /// <returns></returns>
        private static string GetExceptionFullMessage(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }
            var message = new StringBuilder(exception.Message);
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                message.Append("--->");
                message.Append(innerException.Message);
                innerException = innerException.InnerException;
            }
            return message.ToString();
        }
    }
}
