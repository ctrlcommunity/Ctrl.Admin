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
    }
}
