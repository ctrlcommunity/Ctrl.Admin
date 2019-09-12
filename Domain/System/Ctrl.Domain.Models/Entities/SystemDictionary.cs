using Ctrl.Core.PetaPoco;
using System;
using Ctrl.Core.Entities;

namespace Ctrl.System.Models.Entities
{
	/// <summary>
    ///    字典表实体类
    /// </summary>
    [Serializable]
    [TableName("Sys_Dictionary")]
    [PrimaryKey("DictionaryId")]
    public class SystemDictionary: EntityBase
    {
		        /// <summary>
        /// 主键编码
        /// </summary>
        public Guid DictionaryId { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        public bool IsFreeze { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }


    }
}
