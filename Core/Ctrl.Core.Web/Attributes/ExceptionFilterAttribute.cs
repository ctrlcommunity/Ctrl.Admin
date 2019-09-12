using Ctrl.Core.Core.Log;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Ctrl.Core.Web.Attributes
{
    /// <summary>
    ///     异常拦截器
    /// </summary>
    public class ExceptionFilterAttribute : IExceptionFilter
    {
        /// <summary>
        ///   异常发生记录日志
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            ExceptionLogHandler exceptionLogHandler = new ExceptionLogHandler(context.Exception);
            Task.Factory.StartNew(() => exceptionLogHandler.WriteLog());
        }
    }
}
