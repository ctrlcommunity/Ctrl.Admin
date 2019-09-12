using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace Ctrl.System.Business
{
    /// <summary>
    /// 权限记录表业务逻辑接口
    /// </summary>
    public interface ISystemPermissionLogic: IAsyncLogic<SystemPermission>
    {
        ///<summary>
        /// 根据用户id获取具有权限的菜单
        ///</summary>      
        Task<IEnumerable<TreeEntity>>GetSystemPermissionMenuByUserId(Guid userId);
        ///<summary>
        ///     根据权限id查询权限信息
        /// </summary>
        Task<IEnumerable<TreeEntity>>GetPermissionByCheckedPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input);

        ///<summary>
        ///     保存权限信息
        /// </summary>
        Task<OperateStatus>SavePermission(SavePermissionInput input);
        /// <summary>
        /// 根据菜单功获取功能项信息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(IdInput input);


        /// <summary>
        ///     根据登录人员获取对应的路由信息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AuthMenuButtonOutput>> GetRoutersByUserId(string UserId);
        /// <summary>
        ///     根据特权ID获取当前拥有的权限树
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(GetMenuHavePermissionByPrivilegeMasterValueInput input);
        /// <summary>
        ///     根据权限ID查询权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input);

    }
}
