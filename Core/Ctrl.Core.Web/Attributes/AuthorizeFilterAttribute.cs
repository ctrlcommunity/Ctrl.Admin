using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Auth;
using Ctrl.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Ctrl.Core.Web.Attributes
{
    /// <summary>
    ///     执行方法前进入该特性
    /// </summary>
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //获取当前登录人员信息
            PrincipalUser currentUser = AuthenticationExtension.Current();

            #region 是否具有忽略验证特性
            //是否具有忽略特性:若有忽略特性则不进行其他的验证
            if (filterContext.ActionDescriptor.FilterDescriptors.Where(a => a.Filter is IgnoreAttribute).Select(a => a.Filter as IgnoreAttribute).ToList().Count() > 0)
            {
                return;
            }
            #endregion


            #region 用户是否登录
            if (currentUser == null)
            {

                filterContext.Result = new ContentResult()
                {
                    Content = @"<script type='text/javascript'>
                                        alert('登录超时！系统将退出重新登录！');
                                        top.window.location='/sysManage/Account/Login';
                                    </script>",
                    ContentType = "text/html;charset=utf-8;"
                };
                ErrorRedirect(filterContext, "/sysManage/Account/Login");
                return;
            }
            #endregion

            #region 权限验证


            bool isAjaxCall = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (this.SkipAuthorize(filterContext.ActionDescriptor))
            {
                return;
            }
            List<string> permissionCodes = filterContext.ActionDescriptor.FilterDescriptors.Where(a => a.Filter is PermissionAttribute).Select(a => a.Filter as PermissionAttribute).Select(a => a.Code).ToList();

            if (!isAjaxCall)
            {
                if (currentUser != null)
                {
                    if (this.HasExecutePermission(filterContext, permissionCodes))
                    {
                        return;
                    }
                    //说明处于登录状态，则开始验证当前登录用户是否拥有访问权限
                    if (!this.HasExecutePermission(filterContext, filterContext.RouteData.Values["Area"].ToString(), filterContext.RouteData.Values["Controller"].ToString(),
                        filterContext.RouteData.Values["Action"].ToString()))
                    {
                        ErrorRedirect(filterContext, "/sysManage/Home/Unauthorizeds");
                        return;
                    }
                }
            }

            else
            {
                if (permissionCodes.Count == 0)
                {
                    return;
                }
                if (this.HasExecutePermission(filterContext, permissionCodes))
                {
                    return;
                }
                OperateStatus operate = new OperateStatus();
                operate.Message = "抱歉，您无当前操作权限";
                operate.ResultSign = ResultSign.Error;
                ContentResult contentResult = new ContentResult() { Content = JsonConvert.SerializeObject(operate) };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.Result = contentResult;
                return;
            }
            #endregion
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 页面跳转并且需
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url">地址</param>
        private void ErrorRedirect(ActionExecutingContext context, string url)
        {
            context.Result = new RedirectResult(url);
        }
        protected virtual bool SkipAuthorize(ActionDescriptor actionDescriptor)
        {
            bool skipAuthorize = actionDescriptor.FilterDescriptors.Where(a => a.Filter is SkipPermissionAttribute || a.Filter is AllowNotLoginAttribute).Any();
            if (skipAuthorize)
            {
                return true;
            }

            return false;
        }
        protected virtual bool HasExecutePermission(ActionExecutingContext filterContext, string Area, string Controller, string Action)
        {
            return true;
        }

        protected virtual bool HasExecutePermission(ActionExecutingContext filterContext, List<string> permissionCodes)
        {
            return true;
        }

    }
}
