using Ctrl.Core.Entities;
using Ctrl.Core.PetaPoco;
using System;

namespace Ctrl.Domain.Models.Entities
{
    /// <summary>
    ///     Sys_User类
    /// </summary>
    [Serializable]
    [TableName("Sys_User")]
    [PrimaryKey("UserId")]
    public class SystemUser:EntityBase
    {
        /// <summary>
        ///     主键编码
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        ///     登录名
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///     真实姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        ///     电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        ///     第一次访问时间
        /// </summary>
        public DateTime? FirstVisitTime { get; set; }
        /// <summary>
        ///     最后一次访问时间
        /// </summary>
        public DateTime? LastVisitTime { get; set; }
        /// <summary>
        ///     备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        ///     是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        ///     冻结
        /// </summary>
        public bool IsFreeze { get; set; }
        /// <summary>
        ///     头像路径
        /// </summary>
        public string ImgUrl { get; set; }
    }
}
