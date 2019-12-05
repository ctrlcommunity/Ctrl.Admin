using Ctrl.Core.Entities;
using Ctrl.Core.PetaPoco;
using System;

namespace Ctrl.Domain.Models.Entities
{
    /// <summary>
    ///     Sys_ExceptionLog
    /// </summary>
    [Serializable]
    [TableName("Sys_ExceptionLog")]
    [PrimaryKey("ExceptionLogId")]
    public class SystemExceptionLog : EntityBase
    {
        /// <summary>
        ///     主键编码
        /// </summary>
        public string ExceptionLogId { get; set; }
        /// <summary>
        ///  消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        ///     堆栈信息
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        ///     内部信息
        /// </summary>
        public string InnerException { get; set; }
        /// <summary>
        ///     异常类型
        /// </summary>
        public string ExceptionType { get; set; }
        /// <summary>
        ///     服务器
        /// </summary>
        public string ServerHost { get; set; }
        /// <summary>
        ///     客户端
        /// </summary>
        public string ClientHost { get; set; }
        /// <summary>
        ///     运行环境
        /// </summary>
        public string Runtime { get; set; }
        /// <summary>
        ///     请求URL
        /// </summary>
        public string RequestUrl { get; set; }
        /// <summary>
        ///     请求数据
        /// </summary>
        public string RequestData { get; set; }
        /// <summary>
        ///     浏览器代理
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        ///     创建人员
        /// </summary>
        public string CreateUserId { get; set; }

        public string CreateUserCode { get; set; }
        /// <summary>
        ///    创建人员姓名 
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///     客户端地址
        /// </summary>
        public string ClientAddress { get; set; }
    }
}
