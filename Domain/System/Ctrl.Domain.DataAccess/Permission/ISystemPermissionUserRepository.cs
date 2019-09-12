using System;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Domain.Models.Entities;
using Ctrl.Domain.Models.Enums;

namespace Ctrl.System.DataAccess
{
    public interface ISystemPermissionUserRepository: IRepository<SystemPermissionUser>
    {
        /// <summary>
        ///     根据用户Id删除权限用户信息
        /// </summary>
        /// <param name="privilegeMaster"></param>
        /// <param name="privilegeMasterValue"></param>DeletePermissionUser
        /// <returns></returns>     
         Task<bool>DeletePermissionUser(EnumPrivilegeMaster privilegeMaster,Guid privilegeMasterValue);

        /// <summary>
        ///     删除用户
        /// </summary>
        /// <param name="privilegeMasterUserId">用户Id</param>
        /// <param name="privilegeMaster">归属人员类型:组织机构、角色</param>
        /// <returns></returns>
        Task<bool> DeletePrivilegeMasterUser(string privilegeMasterUserId,
            EnumPrivilegeMaster privilegeMaster);
    }
}