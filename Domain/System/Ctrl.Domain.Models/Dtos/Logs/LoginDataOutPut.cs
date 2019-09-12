using Ctrl.Core.Entities.Dtos;

namespace Ctrl.Domain.Models.Dtos.Logs
{
    /// <summary>
    ///     登录日志数据分析图输出类
    /// </summary>
    public class LoginDataOutPut : IOutputDto
    {
        /// <summary>
        ///     省份名称
        /// </summary>
        public string provinceName { get; set; }

        /// <summary>
        ///     登录次数
        /// </summary>
        public int LoginCount { get; set; }
    }
}
