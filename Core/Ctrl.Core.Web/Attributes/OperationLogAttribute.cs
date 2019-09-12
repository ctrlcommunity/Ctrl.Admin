using Ctrl.Core.Core.Auth;
using Ctrl.Core.Core.Http;
using Ctrl.Core.Core.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.ComponentModel;
using System.Linq;

namespace Ctrl.Core.Web.Attributes
{
    /// <summary>
    ///     操作日志记录器
    /// </summary>
    public class OperationLogAttribute : ActionFilterAttribute
    {
        #region 属性
        /// <summary>
        ///     当前登录用户信息
        /// </summary>
        protected virtual PrincipalUser CurrentUser { get; set; }

        /// <summary>
        ///     用户操作日志
        /// </summary>
        private OperationLogHandler _operationLogHandler;

        #endregion

        #region 方法
        /// <summary>
        ///     Action开始执行触发
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _operationLogHandler = new OperationLogHandler(filterContext.HttpContext.Request);
            CurrentUser = AuthenticationExtension.Current();
            if (CurrentUser != null)
            {
                _operationLogHandler.log.CreateUserCode = CurrentUser.Code;
                _operationLogHandler.log.CreateUserName = CurrentUser.Name;
            }

            //获取Action特性
            var descriptionAttribute = filterContext.ActionDescriptor.EndpointMetadata.Where(a => a is DescriptionAttribute).ToList(); ;
            if (descriptionAttribute.Any())
            {
                var info = descriptionAttribute[0] as DescriptionAttribute;
                if (info != null)
                {


                    var description = info.Description;
                    _operationLogHandler.log.ControllerName =
                     ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ControllerName;
                    _operationLogHandler.log.ActionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName;
                    _operationLogHandler.log.Describe = description;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        ///     Action结束执行触发
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (!string.IsNullOrEmpty(_operationLogHandler.log.Describe))
            {
                _operationLogHandler.ActionExecuted();
            }
        }

        /// <summary>
        ///     开始返回结果
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            //记录日志
            if (filterContext.Result.GetType() == typeof(RedirectResult))
            {
                if (!string.IsNullOrEmpty(_operationLogHandler.log.Describe))
                {
                    _operationLogHandler.ActionExecuted();
                }
            }
        }

        /// <summary>
        ///     结果返回结束
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            //记录日志
            if (!string.IsNullOrEmpty(_operationLogHandler.log.Describe))
            {
                _operationLogHandler.ResultExecuted(HttpContexts.Current.Response);
                _operationLogHandler.WriteLog();
            }
        }

        #endregion

    }
}
