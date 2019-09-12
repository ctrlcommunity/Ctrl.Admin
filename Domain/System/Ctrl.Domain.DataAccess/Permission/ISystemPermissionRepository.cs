using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace Ctrl.System.DataAccess
{
     /// <summary>
    /// 权限记录表数据访问接口
    /// </summary>
    public interface ISystemPermissionRepository: IRepository<SystemPermission>
    {
        ///<summary>
        ///     根据权限归属id查询菜单权限信息
        ///</summary>      
        Task<IEnumerable<SystemPermission>>GetPermissionByPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input);
        
        Task<bool>DeletePermissionByPrivilegeMasterValue(EnumPrivilegeAccess?privilegeAccess,
            Guid privilegeMasterValue,Guid privilegeMenuId);
        /// <summary>
        ///     根据用户id获取用户具有的菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetSystemPermissionMenuByUserId(string userId);
        /// <summary>
        ///     根据用户id获取权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<HavePermisionOutput>> GetHavePermisionByUserId(string userId);
        /// <summary>
        ///     根据用户id获取权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetHavePermisionStrByUserId(string userId);
        /// <summary>
        ///     根据角色ID获取具有的菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns>树形菜单信息</returns>
        Task<IEnumerable<TreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(GetMenuHavePermissionByPrivilegeMasterValueInput input);
    }
}