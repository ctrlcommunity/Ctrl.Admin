using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
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
    /// 菜单按钮控制器
    /// </summary>
    public class MenuButtonController : BaseController
    {
        #region 构造函数

        private readonly ISystemMenuButtonLogic _systemMenuButtonLogic;


        public MenuButtonController(ISystemMenuButtonLogic systemMenuButtonLogic)
        {
            _systemMenuButtonLogic = systemMenuButtonLogic;
        }

        #endregion

        #region 视图
        /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-菜单按钮-列表")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-菜单按钮-编辑")]
        [Permission("xtgl-cdan-SaveMenuButton")]
        public async Task<IActionResult> Edit(NullableIdInput input)
        {
            SystemMenuButton menuButton = new SystemMenuButton();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                menuButton=await _systemMenuButtonLogic.GetById(input.Id);
            }
            return View(menuButton);
        }
        #endregion

        #region 方法
        /// <summary>
        ///     获取菜单按钮
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-菜单按钮-方法-获取菜单按钮")]
        public async Task<JsonResult> GetList()
        {
            return Json(await _systemMenuButtonLogic.GetAllEnumerableAsync());
        }
        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("按钮管理-方法-列表-获取按钮列表")]
        public async Task<JsonResult> GetPagingMenuButton(QueryParam queryParam)
        {
            return JsonForGridPaging(await _systemMenuButtonLogic.GetPagingMenuButton(queryParam));
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("按钮管理-方法-新增/编辑-保存")]
        [Permission("xtgl-cdan-SaveMenuButton")]
        public async Task<JsonResult> SaveMenuButton(SystemMenuButton button)
        {
            return Json(await _systemMenuButtonLogic.SaveMenuButton(button));
        }
        /// <summary>
        ///     根据菜单Id获取模块按钮信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("界面按钮-方法-列表-根据菜单ID获取按钮信息")]
        public async Task<JsonResult> GetMenuButtonByMenuId(IdInput input)
        {
            return Json(await _systemMenuButtonLogic.GetMenuButtonByMenuId(input));
        }
        #endregion
    }
}