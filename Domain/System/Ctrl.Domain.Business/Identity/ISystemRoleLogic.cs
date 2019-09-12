using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    /// 角色表业务逻辑接口
    /// </summary>
    public interface ISystemRoleLogic: IAsyncLogic<SystemRole>
    {
        /// <summary>
        ///     保存角色信息
        /// </summary>
        /// <param name="role">角色信息</param>
        /// <returns></returns>
        Task<OperateStatus> SaveRole(SystemRole role);

        /// <summary>
        ///     获取角色分页信息
        /// </summary>
        /// <param name="queryParam">分页信息</param>
        /// <returns></returns>
        Task<PagedResults<SystemRole>> GetPagingSysRole(QueryParam queryParam);
        /// <summary>
        ///     获取角色树
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetAllRoleTree();


    }
}
