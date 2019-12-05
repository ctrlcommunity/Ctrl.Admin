using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Identity;
using Ctrl.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Identity
{
    /// <summary>
    ///     用户业务逻辑
    /// </summary>
    public interface ISystemUserLogic: IAsyncLogic<SystemUser>
    {
        /// <summary>
        ///     根据登录代码和密码查询用户信息
        /// </summary>
        /// <param name="input">用户名、密码等</param>
        /// <returns></returns>
        Task<OperateStatus<UserLoginOutput>> CheckUserByCodeAndPwdAsync(UserLoginInput input);
        /// <summary>
        ///     获取用户列表分页
        /// </summary>
        /// <param name="queryParam">分页参数</param>
        /// <returns></returns>
        Task<PagedResults<SystemUser>> GetPagingSysUser(QueryParam queryParam);

        /// <summary>
        ///     保存人员信息
        /// </summary>
        /// <param name="user">人员信息</param>
        /// <param name="RoleId">业务表Id：如组织机构、角色Id</param>
        /// <returns></returns>
        Task<OperateStatus> SaveUser(SystemUserSaveInput user);
        /// <summary>
        ///     检测代码是否已经具有重复项
        /// </summary>
        /// <param name="input">需要验证的参数</param>
        /// <returns></returns>
        Task<OperateStatus> CheckUserCode(CheckSameValueInput input);
    }
}
