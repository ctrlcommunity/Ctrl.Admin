using Ctrl.Core.Entities.Dtos;
using Ctrl.System.Models.Entities;

namespace Ctrl.Domain.Models.Dtos.Permission
{
    /// <summary>
    ///     按钮输出类
    /// </summary>
    public class SystemMenuButtonOutput:SystemMenuButton,IOutputDto
    {
        /// <summary>
        ///     菜单名称
        /// </summary>
        public string MenuName { get; set; }
    }
}
