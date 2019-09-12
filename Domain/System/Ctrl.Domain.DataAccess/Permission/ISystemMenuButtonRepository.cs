using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
	 /// <summary>
    /// 菜单按钮数据访问接口
    /// </summary>
    public interface ISystemMenuButtonRepository: IRepository<SystemMenuButton>
    {
        /// <summary>
        ///     获取按钮分页
        /// </summary>
        /// <param name="param"></param> 
        /// <returns></returns>
        Task<PagedResults<SystemMenuButtonOutput>> GetPagingMenuButton(QueryParam param);
        /// <summary>
        ///     根据菜单获取功能项信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(IdInput input);
        /// <summary>
        ///     根据用户编码获取权限按钮
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="IsAdmin"></param>
        /// <returns></returns>
        Task<IEnumerable<AuthMenuButtonOutput>> GetMenuButtonByUserId(string UserId, bool IsAdmin);
    }
}
