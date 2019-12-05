using Ctrl.Core.PetaPoco;
using System;
using Ctrl.Core.Entities;

namespace Ctrl.System.Models.Entities
{   
    /// <summary>
     ///    角色表表实体类
     /// </summary>
    [Serializable]
    [TableName("Sys_Role")]
    [PrimaryKey("RoleId")]
    public class SystemRole : EntityBase
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态,临时角色等
        /// </summary>
        public int State { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public bool CanbeDelete { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 冻结
        /// </summary>
        public bool IsFreeze { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建者Id
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 最有一次修改人员时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 最后一次修改人员Id
        /// </summary>
        public Guid UpdateUserId { get; set; }

        /// <summary>
        /// 最后修改人员
        /// </summary>
        public string UpdateUserName { get; set; }


    }
}
