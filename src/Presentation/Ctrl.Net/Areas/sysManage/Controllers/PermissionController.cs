using Ctrl.Core.Web;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Business;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    public class PermissionController : BaseController {

        #region 构造函数
        private readonly ISystemPermissionLogic _permissionLogic;

        public PermissionController(ISystemPermissionLogic permissionLogic){
                this._permissionLogic=permissionLogic;
        }
        #endregion

        #region 方法
        /// <summary>
        ///     根据特权id获取权限信息树
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
         [HttpPost]
         public async Task<JsonResult> GetPermissionByCheckedPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput Input)
         {
            return Json(await _permissionLogic.GetPermissionByCheckedPrivilegeMasterValue(Input)); 
         }   
          /// <summary>
        ///     保存权限
        /// </summary>
        /// <param name="input">权限类型:菜单、模块按钮</param>
        /// <returns></returns>
        [HttpPost]         
        public async Task<JsonResult> SavePermission(SavePermissionInput input)
        {
            if (input.MenuPermissions!=null)
            {
                input.Permissiones = input.MenuPermissions.Split(',');
            }
            
            return Json(await _permissionLogic.SavePermission(input));
        }
        /// <summary>
        ///     根据当前登录用户查询权限按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetRoutersByUserId() {
            return Json(await _permissionLogic.GetRoutersByUserId(CurrentUser.UserId.ToString()));
        }
        /// <summary>
        ///     根据角色id获取具有的菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetMenuHavePermissionByPrivilegeMasterValue(GetMenuHavePermissionByPrivilegeMasterValueInput input)
        {
            return Json(await _permissionLogic.GetMenuHavePermissionByPrivilegeMasterValue(input));
        }
        /// <summary>
        ///     根据权限ID查询权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetPermissionByPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input)
        {
            return Json(await _permissionLogic.GetPermissionByPrivilegeMasterValue(input));
        }
        #endregion
    }
}