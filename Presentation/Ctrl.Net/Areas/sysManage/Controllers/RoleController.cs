using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Converts;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Domain.Business.Config;
using Ctrl.Domain.Models.Dtos.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Ctrl.Core.Web;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Web.Attributes;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 角色表控制器
    /// </summary>
    public class RoleController : BaseController
    {
        #region 构造函数

        private readonly ISystemRoleLogic _systemRoleLogic;


        public RoleController(ISystemRoleLogic systemRoleLogic)
        {
            _systemRoleLogic = systemRoleLogic;
        }

        #endregion

        #region 视图
        /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("角色维护-视图-列表")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///     编辑
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("角色维护-视图-编辑")]
        [Permission("xtgl-jswh-SaveRole")]
        public IActionResult Edit()
        {
            return View();
        }
        /// <summary>
        ///     权限列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("角色维护-视图-选择角色:获取某个人对应的角色信息")]
        [Permission("xtgl-jswh-Chosen")]
        public IActionResult Chosen(IdInput input){
            return View();
        }
        [CreateBy("冯辉")]
        [Description("角色维护-视图-选择按钮:获取当前人和当前菜单对应的按钮信息")]
        [Permission("xtgl-jswh-ChosenButton")]
        public IActionResult ChosenButton() {
            return View();
        }
        #endregion

        #region 方法
        /// <summary>
        ///     保存角色数据
        /// </summary>
        /// <param name="role">角色信息</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("角色维护-方法-新增/编辑-保存")]
        [Permission("xtgl-jswh-SaveRole")]
        public async Task<JsonResult> SaveRole(SystemRole role)
        {
            role.CreateUserId = CurrentUser.UserId;
            role.CreateUserName = CurrentUser.Name;
            return Json(await _systemRoleLogic.SaveRole(role));
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("角色列表-方法-列表-获取角色列表")]
        public async Task<JsonResult> GetPagingRole(QueryParam queryParam)
        {
            return JsonForGridPaging(await _systemRoleLogic.GetPagingSysRole(queryParam));
        }
        
        #endregion
    }
}