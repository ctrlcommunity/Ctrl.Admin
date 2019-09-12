using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
	 /// <summary>
    /// 角色表数据访问接口
    /// </summary>
    public interface ISystemRoleRepository: IRepository<SystemRole>
    {
        /// <summary>
        ///     获取角色分页信息
        /// </summary>
        /// <param name="queryParam">分页信息</param>
        /// <returns></returns>
        Task<PagedResults<SystemRole>> GetPagingSysRole(QueryParam queryParam);

        /// <summary>
        ///     获取所有角色
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetAllRoleTree();
    }
}
