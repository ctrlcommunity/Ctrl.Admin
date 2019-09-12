using Ctrl.Core.Entities;
using Ctrl.Core.PetaPoco;
using System;

namespace Ctrl.Domain.Models.Entities
{
    /// <summary>
    ///     Sys_LoginLog类
    /// </summary>
    [Serializable]
    [TableName("Sys_LoginLog")]
    [PrimaryKey("LoginLogId")]
    public class SystemLoginLog:EntityBase
    {
        /// <summary>
        ///     主键编码
        /// </summary>
        public string LoginLogId { get; set; }
        /// <summary>
        ///     Ip对应地址
        /// </summary>
        public string IpAddressName { get; set; }
        /// <summary>
        ///     服务器主机名
        /// </summary>
        public string ServerHost { get; set; }
        /// <summary>
        ///     客户端主机名
        /// </summary>
        public string ClientHost { get; set; }
        /// <summary>
        ///     浏览器信息
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        ///     操作系统
        /// </summary>
        public string OsVersion { get; set; }
        /// <summary>
        ///     登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        ///     创建人员
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        ///     创建人员登录代码
        /// </summary>
        public string CreateUserCode { get; set; }
        /// <summary>
        ///     创建人员名字
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
