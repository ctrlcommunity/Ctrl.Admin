using Ctrl.Domain.Models.Entities;

namespace Ctrl.Domain.Models.Dtos.Identity
{
    /// <summary>
    ///     用户信息输入类
    /// </summary>
    public class SystemUserSaveInput:SystemUser
    {
        /// <summary>
        ///     角色编码
        /// </summary>
        /// <value></value>
        public string RoleId { get; set; }
    }
}