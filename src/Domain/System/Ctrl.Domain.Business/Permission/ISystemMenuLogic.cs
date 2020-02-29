using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;

namespace Ctrl.System.Business {
    /// <summary>
    /// 系统菜单业务逻辑接口
    /// </summary>
    public interface ISystemMenuLogic:IAsyncLogic<SystemMenu> {
        /// <summary>
        ///     保存菜单
        /// </summary>
        /// <param name="systemMenu"></param>
        /// <returns></returns>
        Task<OperateStatus> SaveMenu (SystemMenu systemMenu);
        /// <summary>
        ///     根据状态为True的菜单信息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetAllMenu ();
        /// <summary>
        /// 根据父级查询下级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemMenuOutput>> GetMenuByPid (IdInput input);
         /// <summary>
        ///   删除菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<OperateStatus>DeleteMenu(IdInput input);
    }
}