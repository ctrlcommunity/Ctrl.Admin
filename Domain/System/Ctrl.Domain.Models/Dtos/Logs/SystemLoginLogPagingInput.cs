using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using System;

namespace Ctrl.Domain.Models.Dtos.Logs
{
    /// <summary>
    ///     登录日志DTO
    /// </summary>
    public  class SystemLoginLogPagingInput: QueryParam, IInputDto
    {
        /// <summary>
        ///     用户名
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        ///     登录代码
        /// </summary>
        public string CreateUserCode { get; set; }
        /// <summary>
        ///     开始日期
        /// </summary>
        public DateTime startTime { get; set; }
        /// <summary>
        ///     结束日期
        /// </summary>
        public DateTime endTime { get; set; }
    }
}
