using Ctrl.Core.Entities.Dtos;

namespace Ctrl.Domain.Models.Dtos.Config
{
    /// <summary>
    ///     表列结构
    /// </summary>
    public class SystemDataBaseColumnOutput: IOutputDto
    {
        /// <summary>
        ///     列名
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        ///     说明
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        ///     宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        ///     对齐方式
        /// </summary>
        public string align { get; set; }
        /// <summary>
        ///     序号
        /// </summary>
        public int OrderNo { get; set; }
        /// <summary>
        ///     类型
        /// </summary>
        public string DataType { get; set; }
    }
}
