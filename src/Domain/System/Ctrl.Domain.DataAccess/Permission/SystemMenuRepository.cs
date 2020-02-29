using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;

namespace Ctrl.System.DataAccess {
    /// <summary>
    ///     系统菜单数据访问接口实现
    /// /// </summary>
    public class SystemMenuRepository : PetaPocoRepository<SystemMenu>, ISystemMenuRepository {
        /// <summary>
        ///     查询所有菜单
        /// </summary>
        /// <param name="haveUrl">是否具有菜单</param>
        /// <param name="isMenuShow">是否菜单显示</param>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetAllMenu (bool haveUrl = false, bool isMenuShow = false) {
            var sql = new StringBuilder ();
            sql.Append (
                "select menu.MenuId id,menu.ParentId pId,menu.name,menu.Code,menu.icon icon");
            if (haveUrl) {
                sql.Append (",menu.OpenUrl url");
            }
            sql.Append (" from Sys_Menu menu");
            if (isMenuShow) {
                sql.Append (" WHERE menu.IsShowMenu=1");
            }
            sql.Append (" ORDER BY menu.OrderNo");
            return SqlMapperUtil.Query<TreeEntity> (sql.ToString ());
        }
        /// <summary>
        ///     根据父级查询下级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemMenuOutput>> GetMenuByPid (IdInput input) {
            var sql = new StringBuilder ();

            sql.Append (@"select menu.*,menu1.name ParentName
                        from sys_menu menu
                        left join Sys_Menu menu1 on menu.ParentId=menu1.menuid where 1=1");
            if (!string.IsNullOrWhiteSpace(input.Id))
                sql.AppendFormat(" AND  menu.ParentId='{0}'", input.Id);
            sql.Append(" ORDER BY menu.menuid");
            return SqlMapperUtil.Query<SystemMenuOutput>(sql.ToString());
        }
    }
}