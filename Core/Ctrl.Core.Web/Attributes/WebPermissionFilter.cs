using Ctrl.Core.Core.Auth;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.DataAccess;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ctrl.Core.Web.Attributes
{
    public class WebPermissionFilter: AuthorizeFilterAttribute
    {
        public const string USER_PERMITS_CACHE_KEY = "_UPERMITS_";
        public const string USER_PERMITSAj_CACHE_KEY = "_UPERMITSAj_";
        IMemoryCache _cache;
        public WebPermissionFilter(IMemoryCache cache)
        {
            this._cache = cache;
        }

        protected override bool HasExecutePermission(ActionExecutingContext filterContext, string Area, string Controller, string Action) {
            var userinfo = AuthenticationExtension.Current();
            if (userinfo.IsAdmin)
                return true;
            else {
                List<HavePermisionOutput> usePermits = null;
                ISystemPermissionRepository systemPermission = new SystemPermissionRepository();
                string cacheKey = USER_PERMITS_CACHE_KEY + userinfo.UserId.ToString();
                usePermits = this._cache.Get<List<HavePermisionOutput>>(cacheKey);
                if (usePermits == null)
                {
                    usePermits = systemPermission.GetHavePermisionByUserId(userinfo.UserId.ToString()).Result.ToList();
                }
                 _cache.Set(cacheKey, usePermits, TimeSpan.FromMinutes(10));//缓存10分钟，10分钟后重新加载
                if (!usePermits.Any(a => Area.Equals(a.Area, StringComparison.OrdinalIgnoreCase)&&Controller.Equals(a.Controller,StringComparison.OrdinalIgnoreCase)&&Action.Equals(a.Action,StringComparison.OrdinalIgnoreCase)))
                    return false;
            }
            return true;
        }

        protected override bool HasExecutePermission(ActionExecutingContext filterContext, List<string> permissionCodes)
        {
            var userinfo = AuthenticationExtension.Current();
            if (userinfo.IsAdmin)
                return true;
            else
            {
                List<string> usePermits = null;
                string userId = userinfo.UserId.ToString();
                string cacheKey = USER_PERMITSAj_CACHE_KEY + userId;
                usePermits = this._cache.Get<List<string>>(cacheKey);
                if (usePermits == null)
                {
                    ISystemPermissionRepository systemPermission = new SystemPermissionRepository();
                    usePermits = systemPermission.GetHavePermisionStrByUserId(userId).Result.ToList();
                    _cache.Set(cacheKey, usePermits, TimeSpan.FromMinutes(15));//缓存15分钟，15分钟后重新加载
                }
                foreach (var permit in permissionCodes)
                {
                    if (!usePermits.Any(a => a == permit))
                        return false;
                }
                return true;
            }
        }
    }
}
