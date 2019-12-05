using Ctrl.Core.Business;
using Ctrl.Core.Core.Resource;
using Ctrl.Core.Entities;
using Ctrl.Domain.Models.Entities;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Permission
{
    /// <summary>
    ///     权限用户业务逻辑
    /// </summary>
    public class SystemPermissionUserLogic : AsyncLogic<SystemPermissionUser>, ISystemPermissionUserLogic
    {
        #region 构造函数
        private readonly ISystemPermissionUserRepository _permissionUserRepository;
        public SystemPermissionUserLogic(ISystemPermissionUserRepository permissionUserRepository):base(permissionUserRepository) {
            this._permissionUserRepository = permissionUserRepository;
        }
        /// <summary>
        ///     删除用户对应权限数据
        /// </summary>
        /// <param name="privilegeMasterUserId">用户Id</param>
        /// <param name="privilegeMaster">归属人员类型:组织机构、角色</param>
        /// <returns></returns>
        public async Task<OperateStatus> DeletePrivilegeMasterUser(string privilegeMasterUserId, EnumPrivilegeMaster privilegeMaster)
        {
            var operateStatus = new OperateStatus();
            if (await _permissionUserRepository.DeletePrivilegeMasterUser(privilegeMasterUserId, privilegeMaster))
            {
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = Chs.Successful;
            }
            return operateStatus;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 保存各种用户:组织机构、人员
        /// </summary>
        /// <param name="master">类型</param>
        /// <param name="value">业务表Id：如组织机构Id、人员Id等</param>
        /// <param name="userids">权限类型:组织机构、人员Id</param>
        /// <returns></returns>
        public async Task<OperateStatus> SavePermissionUser(EnumPrivilegeMaster master, string value, IList<string> userids)
        {
            OperateStatus operateStatus = new OperateStatus();
            IList<SystemPermissionUser> systemPermissionUsers = userids.Select(userId => new SystemPermissionUser
            {
                PrivilegeMaster = (byte)master,
                PrivilegeMasterUserId = userId,
                PrivilegeMasterValue = value
            }).ToList();
            //批量保存
            foreach (var item in systemPermissionUsers)
            {
                operateStatus = await InsertAsync(item);
            }
            return operateStatus;
        }
        #endregion

    }
}
