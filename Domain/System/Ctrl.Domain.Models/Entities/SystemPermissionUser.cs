using System;
using Ctrl.Core.Entities;
using Ctrl.Core.PetaPoco;


namespace Ctrl.Domain.Models.Entities
{
     /// <summary>
    /// System_PermissionUser表实体类
    /// </summary>
    [Serializable]
    [TableName("Sys_PermissionUser")]
    public class SystemPermissionUser:EntityBase
    {
         /// <summary>
        /// 人员归属类型:角色0,组织机构1,人员4(用于查询某用户具有哪些岗位、组等)
        /// </summary>		
        public int PrivilegeMaster { get; set; }

        /// <summary>
        /// 对应类型Id(角色Id,岗位Id,人员Id)
        /// </summary>		
        public string PrivilegeMasterValue { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>		
        public string PrivilegeMasterUserId { get; set; }

    }
}