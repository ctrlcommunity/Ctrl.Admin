using Ctrl.Core.PetaPoco;
using System;
using Ctrl.Core.Entities;

namespace Ctrl.System.Models.Entities
{
	/// <summary>
    ///    文章类型表实体类
    /// </summary>
    [Serializable]
    [TableName("Sys_ArticleType")]
    [PrimaryKey("ArticleTypeId")]
    public class SystemArticleType: EntityBase
    {
		/// <summary>
        /// 主键编码  
        /// </summary>
        public Guid ArticleTypeId { get; set; }

        /// <summary>
        /// 上级编码
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string SeoTitle { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string SeoKey { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string SeoDes { get; set; }
        /// <summary>
        ///     添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }


    }
}
