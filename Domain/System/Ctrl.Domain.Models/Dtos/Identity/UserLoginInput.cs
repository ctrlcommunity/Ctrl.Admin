using Ctrl.Core.Entities.Dtos;

namespace Ctrl.Domain.Models.Dtos
{
    /// <summary>
    ///     用户登录输入实体
    /// </summary>
    public class UserLoginInput:IInputDto
    {
        /// <summary>
        ///     代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///     密码
        /// </summary>
        public string Password { get; set; }
    }
}
