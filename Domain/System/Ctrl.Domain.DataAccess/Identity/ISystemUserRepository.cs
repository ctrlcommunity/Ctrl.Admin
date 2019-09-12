using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Identity;
using Ctrl.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Identity
{
    public interface ISystemUserRepository: IRepository<SystemUser>
    {
        /// <summary>
        ///     根据用户名和密码查询用户信息
        ///     1:用户登录使用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<UserLoginOutput> CheckUserByCodeAndPwd(UserLoginInput input);
        /// <summary>
        ///     获取用户列表分页
        /// </summary>
        /// <param name="queryParam">分页参数</param>
        /// <returns></returns>
        Task<PagedResults<SystemUser>> GetPagingSysUser(QueryParam queryParam);
        /// <summary>
        ///     更新第一次登录时间
        /// </summary>
        /// <param name="input">用户Id</param>
        /// <returns></returns>
        Task<bool> UpdateFirstVisitTime(IdInput input);
        /// <summary>
        ///     更新最后登录时间
        /// </summary>
        /// <param name="input">用户Id</param>
        /// <returns></returns>
        Task<bool> UpdateLastLoginTime(IdInput input);
        /// <summary>
        ///     用户信息修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> UserInfoUpdateSave(UserUpdateInput input);
        /// <summary>
        ///     检测代码是否已经具有重复项
        /// </summary>
        /// <param name="input">需要验证的参数</param>
        /// <returns></returns>
        Task<bool> CheckUserCode(CheckSameValueInput input);
    }
}
