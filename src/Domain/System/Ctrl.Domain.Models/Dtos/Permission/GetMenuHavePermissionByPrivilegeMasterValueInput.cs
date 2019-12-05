using Ctrl.Core.Entities.Dtos;
using Ctrl.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Domain.Models.Dtos.Permission
{
    /// <summary>
    ///  根据角色Id,组Id,人员Id获取具有的菜单信息
    /// </summary>
    public class GetMenuHavePermissionByPrivilegeMasterValueInput : IInputDto
    {
        /// <summary>
        /// 根据角色Id,组Id,人员Id
        /// </summary>
        public string PrivilegeMasterValue { get; set; }

        /// <summary>
        /// 权限类型:角色、组、人员
        /// </summary>
        public EnumPrivilegeMaster PrivilegeMaster { get; set; }

        /// <summary>
        /// 权限归属:菜单,功能项,字段,数据权限
        ///     需要排除菜单信息:不在此类型范围类的不给与显示
        /// </summary>
        public EnumPrivilegeAccess? PrivilegeAccess { get; set; }
    }
}
