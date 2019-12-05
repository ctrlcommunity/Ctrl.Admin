using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.Domain.Models.Entities;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.Models.Entities;

namespace Ctrl.System.DataAccess {
    /// <summary>
    ///     权限记录表数据访问接口实现
    /// </summary>
    public class SystemPermissionUserRepository : PetaPocoRepository<SystemPermissionUser>, ISystemPermissionUserRepository
    {
        /// <summary>
        ///     根据特性id删除权限用户信息
        /// </summary>
        /// <param name="privilegeMaster">特性类型：组织机构、角色等</param>
        /// <param name="privilegeMasterValue">特性类型：组织机构、角色等</param>
        /// <returns></returns>
        public Task<bool> DeletePermissionUser(EnumPrivilegeMaster privilegeMaster, Guid privilegeMasterValue)
        {
            const string sql = "DELETE FROM Columns WHERE PrivilegeMaster=@privilegeMaster AND PrivilegeMasterValue=@privilegeMasterValue";
             return SqlMapperUtil.InsertUpdateOrDeleteSqlBool<SystemPermissionUser>(sql,
                new
                {
                    privilegeMaster = (byte)privilegeMaster,
                    privilegeMasterValue
                });
        }
        /// <summary>
        ///     删除用户
        /// </summary>
        /// <param name="privilegeMasterUserId">用户Id</param>
        /// <param name="privilegeMaster">归属人员类型:组织机构、角色</param>
        /// <returns></returns>
        public Task<bool> DeletePrivilegeMasterUser(string privilegeMasterUserId, EnumPrivilegeMaster privilegeMaster)
        {
            const string sql = "DELETE FROM System_PermissionUser WHERE PrivilegeMaster=@privilegeMaster AND PrivilegeMasterUserId=@privilegeMasterUserId";
            return SqlMapperUtil.InsertUpdateOrDeleteSqlBool<SystemPermissionUser>(sql,
               new { privilegeMaster = (byte)privilegeMaster, privilegeMasterUserId });
        }
    }
}