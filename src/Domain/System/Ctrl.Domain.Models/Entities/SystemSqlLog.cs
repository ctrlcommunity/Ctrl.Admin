using Ctrl.Core.PetaPoco;
using System;
using Ctrl.Core.Entities;

namespace Ctrl.System.Models.Entities
{
	/// <summary>
    ///    sql执行日志表表实体类
    /// </summary>
    [Serializable]
    [TableName("Sys_SqlLog")]
    [PrimaryKey("SqlLogId")]
    public class SystemSqlLog: EntityBase
    {
		        /// <summary>
        /// sql日志Id
        /// </summary>
        public string SqlLogId { get; set; }

        /// <summary>
        /// 创建人员
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建人员登录代码
        /// </summary>
        public string CreateUserCode { get; set; }

        /// <summary>
        /// 创建人员名字
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 操作sql
        /// </summary>
        public string OperateSql { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public decimal ElapsedTime { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }


    }
}
