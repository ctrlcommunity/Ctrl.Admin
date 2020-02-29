using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Web;
using Ctrl.Core.Web.Attributes;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;


namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 系统菜单控制器
    /// </summary>
    public class  MenuController : BaseController
    {
        #region 构造函数

        private readonly ISystemMenuLogic _systemMenuLogic;


        public MenuController(ISystemMenuLogic systemMenuLogic)
        {
            _systemMenuLogic = systemMenuLogic;
        }

        #endregion

        #region 视图
		 /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-系统菜单-列表")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///     注：非前后端分离,采用服务端渲染视图 
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-系统菜单-编辑")]
        [Permission("xtgl-mkwh-SaveMenu")]
        public async Task<IActionResult> Edit(NullableIdInput input) {
            SystemMenu menu = new SystemMenu();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                // menu =await _systemMenuLogic.GetById(input.Id);
            }
         
            return View(menu);
        }
        #endregion

        #region 方法
        /// <summary>
        ///     获取系统菜单
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("模块维护-方法-获取系统菜单")]
        public async Task<JsonResult> GetList()
        {
            return null;
            // return Json(await _systemMenuLogic.GetAllEnumerableAsync());
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [ValidateAntiForgeryToken]
        [Description("模块维护-方法-新增/编辑-保存")]
        [Permission("xtgl-mkwh-SaveMenu")]
        public async Task<JsonResult> SaveMenu(SystemMenu systemMenu) {
            return Json(await _systemMenuLogic.SaveMenu(systemMenu));
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("模块维护-方法-列表根据父级Id获取下级菜单")]
        public async Task<JsonResult>GetMeunuByPId(IdInput input){
            return Json(await _systemMenuLogic.GetMenuByPid(input));
        }
        /// <summary>
        ///     删除菜单
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [CreateBy("冯辉")]
        [Description("模块维护-方法-删除菜单")]
        [Permission("xtgl-mkwh-DeleteMenu")]
        public async Task<JsonResult>DeleteMenu(IdInput input){
            return Json(await _systemMenuLogic.DeleteMenu(input));

        }
		#endregion
	}
}