using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     菜单按钮数据访问接口实现
    /// </summary>
    public class SystemMenuButtonRepository : PetaPocoRepository<SystemMenuButton>, ISystemMenuButtonRepository
    {
        /// <summary>
        ///     根据菜单项获取功能项信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(IdInput input)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT f.*,menu.Name MenuName FROM Sys_MenuButton f LEFT JOIN Sys_Menu menu ON menu.MenuId=f.MenuId WHERE 1=1");
            if (!string.IsNullOrWhiteSpace(input.Id))
            {
                sb.AppendFormat(" AND f.MenuId='{0}'", input.Id);
            }
            return SqlMapperUtil.Query<SystemMenuButtonOutput>(sb.ToString());
        }
        /// <summary>
        ///     根据用户编码获取权限按钮
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="IsAdmin"></param>
        /// <returns></returns>
        public Task<IEnumerable<AuthMenuButtonOutput>> GetMenuButtonByUserId(string UserId, bool IsAdmin)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"	SELECT
	                            func.* ,menu.Area,menu.Controller,menu.Action
                            FROM
	                            sys_menubutton func
	                            LEFT JOIN sys_menu menu ON func.MenuId = menu.MenuId
	                            LEFT JOIN sys_permission sper ON sper.PrivilegeAccessValue = func.MenuButtonId
	                            LEFT JOIN sys_permissionuser spuser ON sper.PrivilegeMasterValue = spuser.PrivilegeMasterValue
	                            WHERE 1=1 ");
            if (!IsAdmin)
            {
                sql.AppendFormat(" and spuser.PrivilegeMasterUserId='{0}'", UserId);
            }
            sql.Append("	order by OrderNo desc");
            return SqlMapperUtil.Query<AuthMenuButtonOutput>(sql.ToString());
        }

        /// <summary>
        ///     获取按钮分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemMenuButtonOutput>> GetPagingMenuButton(QueryParam param)
        {
            string sql = @"select button.*,menu.Name menuname
                        from Sys_MenuButton button
                        left join Sys_Menu menu on button.MenuId=menu.MenuId";
            return SqlMapperUtil.PagingQuery<SystemMenuButtonOutput>(sql, param);
        }
    }
}
