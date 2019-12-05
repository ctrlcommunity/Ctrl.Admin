using Ctrl.Core.Entities.Dtos;
using Ctrl.Domain.Models.Enums;
using System.Collections.Generic;

namespace Ctrl.Domain.Models.Dtos.Config
{
    public class SystemCodeGenerationInput: SystemDataBaseTableOutput
    {
        /// <summary>
        ///     基础
        /// </summary>
        public SystemCodeGenerationBaseInput Base { get; set; }

        /// <summary>
        ///     列表
        /// </summary>
        public IEnumerable<SystemCodeGenerationListForListInput> List { get; set; }

        /// <summary>
        ///     编辑
        /// </summary>
        public IEnumerable<SystemCodeGenerationEditInput> Edit { get; set; }
    }
    public class SystemCodeGenerationBaseInput : SystemDataBaseTableOutput
    {
        /// <summary>
        ///     主键
        /// </summary>
        public string TableKey { get; set; }

        /// <summary>
        ///     实体类名
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        ///     DataAccess接口
        /// </summary>
        public string DataAccessInterface { get; set; }

        /// <summary>
        ///     DataAccess实现
        /// </summary>
        public string DataAccess { get; set; }

        /// <summary>
        ///     Business接口
        /// </summary>
        public string BusinessInterface { get; set; }

        /// <summary>
        ///     Business实现
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        ///     控制器
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        ///     列表页名
        /// </summary>
        public string List { get; set; }

        /// <summary>
        ///     表单页名
        /// </summary>
        public string Edit { get; set; }

        /// <summary>
        ///     实体路径
        /// </summary>
        public string EntityPath { get; set; }

        /// <summary>
        ///     DataAccess接口路径
        /// </summary>
        public string DataAccessInterfacePath { get; set; }

        /// <summary>
        ///     DataAccess实现路径
        /// </summary>
        public string DataAccessPath { get; set; }

        /// <summary>
        ///     Business接口路径
        /// </summary>
        public string BusinessInterfacePath { get; set; }

        /// <summary>
        ///     Business实现路径
        /// </summary>
        public string BusinessPath { get; set; }

        /// <summary>
        ///     控制器
        /// </summary>
        public string ControllerPath { get; set; }
        /// <summary>
        ///     是否分页
        /// </summary>
        public bool IsPaging { get; set; }

        /// <summary>
        /// 编辑框宽
        /// </summary>
        public int EditWidth { get; set; }

        /// <summary>
        /// 编辑框高
        /// </summary>
        public int EditHeight { get; set; }
    }
    public class SystemCodeGenerationListForListInput : IInputDto
    {
        /// <summary>
        ///     字段名称
        /// </summary>
        public string field { get; set; }

        /// <summary>
        ///     显示名称
        /// </summary>
        public string title { get; set; }

        /// <summary>
        ///     排序名称
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        ///     显示列宽
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     对齐方式
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        ///     是否显示
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        ///     自定义转换
        /// </summary>
        public string Formatter { get; set; }

        /// <summary>
        ///     排序类型
        /// </summary>
        public string Sorttype { get; set; }

        /// <summary>
        ///     排序
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        ///     是否排序
        /// </summary>
        public bool Sort { get; set; }
    }
    /// <summary>
    ///     编辑界面信息
    /// </summary>
    public class SystemCodeGenerationEditInput : IInputDto
    {
        /// <summary>
        ///     控件Id/Name
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        ///     控件名称
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        ///     默认值
        /// </summary>
        public string ControlDefault { get; set; }
        /// <summary>
        ///     字段类型
        /// </summary>
        public EnumControlType ControlType { get; set; }
    }
}
