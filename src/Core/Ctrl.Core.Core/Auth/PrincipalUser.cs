using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Ctrl.Core.Core.Auth
{
    /// <summary>
    ///     页面中需要的用户信息类,继承用户对象的基本功能(.net验证机制对象IPrincipal和IIdentity)
    /// </summary>
    public class PrincipalUser : ClaimsPrincipal
    {
        #region 基础实体

        /// <summary>
        ///     主键Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     用户姓名
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string ContactNumber { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 登录后的Id值,退出时更新退出时间
        /// </summary>
        public Guid LoginId { get; set; }
        /// <summary>
        ///     是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        ///     头像地址
        /// </summary>
        public string ImgUrl{ get; set; }

        //public IIdentity Identity => null;

        //public bool IsInRole(string role)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
