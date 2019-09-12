using Ctrl.Core.Entities.Dtos;
using Ctrl.System.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Domain.Models.Dtos.Permission
{
    public class AuthMenuButtonOutput:SystemMenuButton,IOutputDto
    {
        /// <summary>
        ///     区域
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        ///     控制器
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        ///     方法
        /// </summary>
        public string Action { get; set; }
    }
}
