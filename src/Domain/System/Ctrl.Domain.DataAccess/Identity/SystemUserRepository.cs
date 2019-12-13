using Ctrl.Core.Core.Utils;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Identity;
using Ctrl.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Identity
{
    public class SystemUserRepository : PetaPocoRepository<SystemUser>, ISystemUserRepository
    {
        /// <summary>
        ///     根据用户名和密码查询用户信息
        ///     1:用户登录使用
        /// </summary>
        /// <param name="input">用户名、密码等</param>
        /// <returns></returns>
        public Task<UserLoginOutput> CheckUserByCodeAndPwd(UserLoginInput input)
        {
            var sql= @"select sysUser.UserId,sysUser.Code,sysUser.Name,sysUser.IsAdmin,role.Name RoleName,sysUser.IsFreeze,sysUser.FirstVisitTime,sysUser.ImgUrl  from Sys_User sysUser
                    left join Sys_PermissionUser per on sysUser.UserId=per.PrivilegeMasterUserId
                    left join Sys_Role role on  role.RoleId=per.PrivilegeMasterValue
                    where sysUser.Code=@Code and sysUser.Password=@pwd";
            return SqlMapperUtil.FirstOrDefault<UserLoginOutput>(sql,new { Code=input.Code,pwd=input.Password});
        }
        /// <summary>
        ///     获取用户列表
        /// </summary>
        /// <param name="queryParam">分页参数</param>
        /// <returns></returns>
        public Task<PagedResults<SystemUser>> GetPagingSysUser(QueryParam queryParam)
        {
            var sql = "SELECT * FROM [dbo].[Sys_User] sysUser";
            return SqlMapperUtil.PagingQuery<SystemUser>(sql,queryParam);
        }

        /// <summary>
        ///     更新最后登录时间
        /// </summary>
        /// <param name="input">用户Id</param>
        /// <returns></returns>
        public Task<bool> UpdateLastLoginTime(IdInput input)
        {
            const string sql = @"UPDATE [Sys_User] SET LastVisitTime=getdate() WHERE UserId=@userId";
            return SqlMapperUtil.InsertUpdateOrDeleteSqlBool(sql, new { userId = input.Id });
        }

        /// <summary>
        ///     更新第一次登录时间
        /// </summary>
        /// <param name="input">用户Id</param>
        /// <returns></returns>
        public Task<bool> UpdateFirstVisitTime(IdInput input)
        {
            const string sql = @"UPDATE [Sys_User] SET FirstVisitTime=getdate() WHERE UserId=@userId";
            return SqlMapperUtil.InsertUpdateOrDeleteSqlBool(sql, new { userId = input.Id });
        }
        /// <summary>
        ///     用户信息修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> UserInfoUpdateSave(UserUpdateInput input)
        {
            const string sql = @"UPDATE [Sys_User] 
                                SET ImgUrl=@ImgUrl,
                                Name=@Name
                                WHERE UserId=@userId";
            return SqlMapperUtil.InsertUpdateOrDeleteSqlBool(sql,input);
        }
        /// <summary>
        ///     检测代码是否已经具有重复项
        /// </summary>
        /// <param name="input">需要验证的参数</param>
        /// <returns></returns>
        public Task<bool> CheckUserCode(CheckSameValueInput input)
        {
            string sql = "select UserId from Sys_User where Code=@param";
            if (!input.Id.IsNullOrEmptyGuid())
            {
                sql += " And UserId!=@UserId";
            }
            return SqlMapperUtil.SqlWithParamsBool<SystemUser>(sql,new { param=input.Param, UserId=input.Id});
        }
    }
}
