using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    /// 菜单按钮业务逻辑接口
    /// </summary>
    public interface ISystemMenuButtonLogic: IAsyncLogic<SystemMenuButton>
    {
        /// <summary>
        ///     获取按钮分页信息
        /// </summary>
        /// <param name="queryParam">分页信息</param>
        /// <returns></returns>
        Task<PagedResults<SystemMenuButtonOutput>> GetPagingMenuButton(QueryParam param);

        /// <summary>
        ///     保存功能项信息
        /// </summary>
        /// <param name="menuButton">功能项信息</param>
        /// <returns></returns>
        Task<OperateStatus> SaveMenuButton(SystemMenuButton menuButton);
        /// <summary>
        ///     根据菜单获取功能项信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(IdInput input);
    }
}
