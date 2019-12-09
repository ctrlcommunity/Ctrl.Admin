using Ctrl.Core.Core.Auth;
using Ctrl.Core.Entities.Paging;
using Microsoft.AspNetCore.Mvc;

namespace Ctrl.Core.Web
{
    //[AuthorizeFilter]
    [Area(areaName: "sysManage")]
    public class BaseController: Controller
    {
        #region Json
        /// <summary>
        ///     返回分页后信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedResults"></param>
        /// <returns></returns>
        protected JsonResult JsonForGridPaging<T>(PagedResults<T> pagedResults)
        {
            return Json(new
            {
                total = pagedResults.pagerInfo.PageCount,
                page = pagedResults.pagerInfo.Pageindex,
                records = pagedResults.pagerInfo.RecordCount,
                rows = pagedResults.Data
            });
        }
        #endregion

        #region 属性

        /// <summary>
        ///     当前登录用户信息
        /// </summary>
        protected  virtual PrincipalUser CurrentUser { get {
                return AuthenticationExtension.Current();
            } }

        #endregion
    }
}
