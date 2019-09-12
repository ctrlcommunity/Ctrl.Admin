using Ctrl.Core.Entities.Dtos;

namespace Ctrl.Domain.Models.Dtos.Identity
{
    /// <summary>
    ///     用户资料修改输入类
    /// </summary>
    public class UserUpdateInput: IInputDto
    {
        /// <summary>
        ///     头像地址
        /// </summary>
        public string ImgUrl { get; set;}
        /// <summary>
        ///     登录代码
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     用户编码
        /// </summary>
        public string userId { get; set; }
    }
}
