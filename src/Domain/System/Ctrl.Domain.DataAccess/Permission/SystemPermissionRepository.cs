using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.Models.Entities;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     权限记录表数据访问接口实现
    /// </summary>
    public class SystemPermissionRepository : PetaPocoRepository<SystemPermission>, ISystemPermissionRepository
    {

        /// <summary>
        ///     根据角色id获取具有的菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns>树形菜单信息</returns>
        public Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input)
        {
            var sql = new StringBuilder($"select * from Sys_Permission where PrivilegeAccess=@privilegeAccess");
            sql.Append(" AND PrivilegeMasterValue=@privilegeMasterValue ");
            if (!string.IsNullOrWhiteSpace(input.PrivilegeMenuId))
            {
                sql.Append(@" AND  PrivilegeAccessValue in(select MenuButtonId from 
                        sys_menubutton where MenuId=@privilegeMenuId)");
            }
            return SqlMapperUtil.Query<SystemPermission>(sql.ToString(), new
            {
                privilegeAccess = (byte)input.PrivilegeAccess,
                privilegeMasterValue = input.PrivilegeMasterValue
                ,privilegeMenuId = input.PrivilegeMenuId
            });
        }
        /// <summary>
        ///     根据权限归属Id删除菜单权限信息
        /// </summary>
        /// <param name="privilegeAccess">权限类型:菜单、功能项</param>
        /// <param name="privilegeMasterValue"></param>
        /// <param name="privilegeMenuId"></param>
        /// <returns></returns>
        public Task<bool> DeletePermissionByPrivilegeMasterValue(EnumPrivilegeAccess? privilegeAccess,
            Guid privilegeMasterValue, Guid privilegeMenuId)
        {
            var deleteSql = new StringBuilder("delete from sys_Permission where privilegeMasterValue=@privilegeMasterValue");
            if (privilegeAccess != null)
            {
                deleteSql.Append(" AND PrivilegeAccess=@PrivilegeAccess");
                if (privilegeMenuId != null)
                {
                    deleteSql.Append(" AND privilegeMenuId=@privilegeMenuId");
                }
                return SqlMapperUtil.InsertUpdateOrDeleteSqlBool
                    (deleteSql.ToString(), new { PrivilegeAccess = (int)privilegeAccess, privilegeMasterValue, privilegeMenuId });
            }
            return SqlMapperUtil.InsertUpdateOrDeleteSqlBool(deleteSql.ToString(), new { privilegeMasterValue });
        }
        /// <summary>
        ///     根据用户id获取用户具有的菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetSystemPermissionMenuByUserId(string userId)
        {
            var sql = new StringBuilder(@"SELECT
	                                            menu.MenuId id,menu.ParentId pId,menu.name,menu.OpenUrl url,menu.icon   
                                            FROM
	                                            SYS_MENU menu
	                                            LEFT JOIN sys_permission sper ON sper.PrivilegeAccessValue = menu.MenuId
	                                            LEFT JOIN sys_permissionuser speruser ON sper.PrivilegeMasterValue = speruser.PrivilegeMasterValue 
                                            WHERE
	                                            sper.PrivilegeAccess = @privilegeAccess 
	                                            AND menu.IsShowMenu = @isShowMenu 
	                                            AND menu.IsFreeze = @isFreeze 
	                                            AND speruser.PrivilegeMasterUserId=@userId 
                                            GROUP BY
	                                            menu.MenuId,
	                                            menu.ParentId,
	                                            menu.NAME,
	                                            menu.OpenUrl,
	                                            menu.OrderNo,
                                                menu.icon 
                                            ORDER BY
	                                            menu.OrderNo");
            return SqlMapperUtil.Query<TreeEntity>(sql.ToString(),
                new { privilegeAccess = (byte)EnumPrivilegeAccess.菜单, isShowMenu = true, isFreeze = false, userId });
        }
        /// <summary>
        ///     根据用户id获取权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<IEnumerable<HavePermisionOutput>> GetHavePermisionByUserId(string userId)
        {
            string sql = @"SELECT
	                                            menu.Area,menu.Controller,menu.Action,menu.OrderNo,menu.MenuId
                                            FROM
	                                            SYS_MENU menu
	                                            LEFT JOIN sys_permission sper ON sper.PrivilegeAccessValue = menu.MenuId
	                                            LEFT JOIN sys_permissionuser speruser ON sper.PrivilegeMasterValue = speruser.PrivilegeMasterValue 
                                            WHERE
	                                            sper.PrivilegeAccess = @privilegeAccess 
	                                            AND menu.IsShowMenu = @isShowMenu 
	                                            AND menu.IsFreeze = @isFreeze 
	                                            AND speruser.PrivilegeMasterUserId=@userId 
                                            GROUP BY
	                                            menu.MenuId,
	                                            menu.Area,
	                                            menu.Controller,
	                                            menu.Action,
	                                            menu.OrderNo 
                                            ORDER BY
	                                            menu.OrderNo";
            return SqlMapperUtil.Query<HavePermisionOutput>(sql.ToString(),
      new { privilegeAccess = (byte)EnumPrivilegeAccess.菜单, isShowMenu = true, isFreeze = false, userId });
        }
        /// <summary>
        ///     根据用户id获取权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetHavePermisionStrByUserId(string userId)
        {
            var sql = new StringBuilder(@"SELECT
	                                            menu.Code
                                            FROM
	                                            Sys_MenuButton menu
	                                            LEFT JOIN Sys_permission sper ON sper.PrivilegeAccessValue = menu.MenuButtonId
	                                            LEFT JOIN Sys_permissionuser speruser ON sper.PrivilegeMasterValue = speruser.PrivilegeMasterValue 
												where
                                               sper.PrivilegeAccess = @privilegeAccess 
	                                            AND speruser.PrivilegeMasterUserId=@userId 
                                            GROUP BY
												menu.Code
                      ");
            return SqlMapperUtil.SqlWithParams<string>(sql.ToString(),
                new { privilegeAccess = (byte)EnumPrivilegeAccess.菜单按钮, userId });
        }

        /// <summary>
        ///     根据角色ID获取具有的菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns>树形菜单信息</returns>
        public Task<IEnumerable<TreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(GetMenuHavePermissionByPrivilegeMasterValueInput input)
        {
            var sql = new StringBuilder(
                @"SELECT MenuId id,ParentId pId,name,icon FROM Sys_Menu menu WHERE MenuId IN( SELECT PrivilegeAccessValue  FROM Sys_Permission WHERE PrivilegeAccess=@privilegeAccess AND PrivilegeMasterValue=@privilegeMasterValue GROUP BY PrivilegeAccessValue)
order by OrderNo");
            return SqlMapperUtil.SqlWithParams<TreeEntity>(sql.ToString(),
              new
              {
                  privilegeAccess = EnumPrivilegeAccess.菜单,
                  privilegeMasterValue = input.PrivilegeMasterValue,
                  isFreeze = false
              });

        }
    }
}