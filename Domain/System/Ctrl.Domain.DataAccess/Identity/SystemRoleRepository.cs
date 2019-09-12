using System.Collections.Generic;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.System.Models.Entities;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     角色表数据访问接口实现
    /// </summary>
    public class SystemRoleRepository : PetaPocoRepository<SystemRole>, ISystemRoleRepository
    {
        /// <summary>
        ///     获取所有角色
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetAllRoleTree()
        {
            var sql = @"select  roles.Name,roles.RoleId id
                        from Sys_Role roles";
            return SqlMapperUtil.Query<TreeEntity>(sql);
        }

        /// <summary>
        ///     获取角色分页数据
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemRole>> GetPagingSysRole(QueryParam queryParam)
        {
            var sql = "SELECT * FROM Sys_Role";
            return SqlMapperUtil.PagingQuery<SystemRole>(sql, queryParam);
        }
    }
}
