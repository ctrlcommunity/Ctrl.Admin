using Ctrl.Core.Business;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.System.DataAccess;
using Ctrl.System.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    ///     角色表业务逻辑接口实现
    /// /// </summary>
    public class SystemRoleLogic:AsyncLogic<SystemRole>,ISystemRoleLogic
    {
        #region 构造函数
        private readonly ISystemRoleRepository _systemRoleRepository;

        public SystemRoleLogic(ISystemRoleRepository systemRoleRepository):base(systemRoleRepository) {
            _systemRoleRepository = systemRoleRepository;
        }
        #endregion

        #region 方法
        /// <summary>
        ///     保存角色信息
        /// </summary>
        /// <param name="role">角色信息</param>
        /// <returns></returns>
        public  Task<OperateStatus> SaveRole(SystemRole role)
        {
            if (role.RoleId.IsEmptyGuid())
            {
                role.CreateTime = DateTime.Now;
                role.RoleId = Guid.NewGuid();
                return InsertAsync(role);
            }
            return null;
        }
        /// <summary>
        ///     获取角色分页
        /// </summary>
        /// <param name="queryParam">分页信息</param>
        /// <returns></returns>
        public  Task<PagedResults<SystemRole>> GetPagingSysRole(QueryParam queryParam)
        {
            return  _systemRoleRepository.GetPagingSysRole(queryParam);
        }
        /// <summary>
        ///     获取角色树
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetAllRoleTree()
        {
            return _systemRoleRepository.GetAllRoleTree();
        }
        #endregion
    }
}