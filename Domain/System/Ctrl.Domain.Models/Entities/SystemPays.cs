using Ctrl.Core.PetaPoco;
using System;
using Ctrl.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ctrl.System.Models.Entities
{
	/// <summary>
    ///     支付配置表表实体类
    /// </summary>
    [Serializable]
    [TableName("Sys_Pays")]
    [PrimaryKey("PayId")]
    public class SystemPays: EntityBase
    {
		        /// <summary>
        /// 支付主键
        /// </summary>
        public Guid PayId { get; set; }

        /// <summary>
        ///  支付名称
        /// </summary>
        public string PayName { get; set; }

        /// <summary>
        /// 支付logo
        /// </summary>
        public string PayLogo { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///     配置信息
        /// </summary>
        public string ConfigInfo { get; set; }
        /// <summary>
        ///     支付类型
        /// </summary>
        public string PayType { get; set; }

    }
}
