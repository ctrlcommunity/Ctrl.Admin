using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Web;
using Ctrl.System.Business;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using Ctrl.Core.Web.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Ctrl.Net.Areas.SysManage.Controllers
{
    /// <summary>
    ///     首页
    /// </summary>
    public class HomeController : BaseController
    {

        #region 构造函数
        private readonly ISystemPermissionLogic _permissionLogic;
        public HomeController(ISystemPermissionLogic permissionLogic)
        {
            this._permissionLogic = permissionLogic;
        }

        #endregion

        #region 视图
        /// <summary>
        ///     首页
        /// </summary>
        [Route(""), Route("/SysManage/Home/Index"), Route("/SysManage")]
        [CreateBy("冯辉")]
        [Description("首页-界面")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     处理404
        /// </summary>
        /// <returns></returns>
        [SkipPermission]
        public ActionResult NotFounds(){
            return View();
        }
        /// <summary>
        ///     处理401未授权
        /// </summary>
        /// <returns></returns>
        [SkipPermission]
        public ActionResult Unauthorizeds() {
            return View();
        }
        #endregion

        #region 方法 
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("首页-方法-获取菜单模块权限")]
        public async Task<JsonResult>LoadMenuPermission(){
            var data=(await _permissionLogic.GetSystemPermissionMenuByUserId(CurrentUser.UserId)).ToList();
            return Json(data);
        }
     
        #endregion


    }
}