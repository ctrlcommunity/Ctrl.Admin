using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Domain.Models.Entities;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Permission
{
    /// <summary>
    ///     权限用户业务逻辑接口
    /// </summary>
    public interface ISystemPermissionUserLogic: IAsyncLogic<SystemPermissionUser>
    {

        /// <summary>
        /// 保存各种用户:组织机构、人员
        /// </summary>
        /// <param name="master">类型</param>
        /// <param name="value">业务表Id：如组织机构Id、人员Id等</param>
        /// <param name="userids">权限类型:组织机构、人员Id</param>
        /// <returns></returns>
        Task<OperateStatus> SavePermissionUser(EnumPrivilegeMaster master, string value, IList<string> userids);

        /// <summary>
        ///     删除用户对应权限数据
        /// </summary>
        /// <param name="privilegeMasterUserId">用户Id</param>
        /// <param name="privilegeMaster">归属人员类型:组织机构、角色</param>
        /// <returns></returns>
        Task<OperateStatus> DeletePrivilegeMasterUser(string privilegeMasterUserId, EnumPrivilegeMaster privilegeMaster);
    }
}
