using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    /// 系统菜单数据访问接口
    /// </summary>
    public interface ISystemMenuRepository: IRepository<SystemMenu>
    {
        /// <summary>
        ///     查询所有菜单
        /// </summary>
        /// <param name="haveUrl">是否具有菜单</param>
        /// <param name="isMenuShow"></param>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetAllMenu(bool haveUrl = false,
            bool isMenuShow = false);

        ///<summary>
        /// 根据父级查询下级
        /// </summary>
        Task<IEnumerable<SystemMenuOutput>> GetMenuByPid(IdInput input);
    }
}
