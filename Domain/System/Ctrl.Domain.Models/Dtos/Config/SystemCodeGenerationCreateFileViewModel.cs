namespace Ctrl.Domain.Models.Dtos.Config
{
    /// <summary>
    /// 创建文件
    /// </summary>
    public class SystemCodeGenerationCreateFileViewModel : SystemCodeGenerationOutput
    {
        /// <summary>
        /// 基础参数
        /// </summary>
        public string Base { get; set; }
        /// <summary>
        ///     列表
        /// </summary>
        public string List { get; set; }
        /// <summary>
        ///     编辑
        /// </summary>
        public string Edit { get; set; }
    }
}
