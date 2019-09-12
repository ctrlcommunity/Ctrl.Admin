using System;
using System.Collections.Generic;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Domain.Models.Enums;

namespace Ctrl.Domain.Models.Dtos.Permission
{
    public class SavePermissionInput:IInputDto
    {
        /// <summary>
        ///     添加类型：菜单、功能项
        /// </summary>
        public EnumPrivilegeAccess PrivilegeAccess { get; set; }
        /// <summary>
        /// 添加的类型:角色、组织机构、人员
        /// </summary>
        public EnumPrivilegeMaster PrivilegeMaster { get; set; }
        /// <summary>
        ///     权限信息
        /// </summary>
        public IList<string> Permissiones { get; set; }
        /// <summary>
        ///     角色、组织机构、人员主键ID
        /// </summary>
        public Guid PrivilegeMasterValue { get; set; }
        /// <summary>
        /// 对应菜单Id
        /// </summary>
        public Guid PrivilegeMenuId { get; set; }
        /// <summary>
        ///     权限信息
        /// </summary>
        public string MenuPermissions { get; set; }
    }
}