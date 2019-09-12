using System;

namespace Ctrl.Core.Core.Log
{
    /// <summary>
    ///     登录日志
    /// </summary>
    public class LoginLog
    {
        /// <summary>
        ///     主键Id
        /// </summary>
        public string LoginLogId { get; set; }

        /// <summary>
        ///     Ip对应地址
        /// </summary>
        public string IpAddressName { get; set; }

        /// <summary>
        ///     服务器主机名
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        ///     客户端主机名
        /// </summary>
        public string ClientHost { get; set; }

        /// <summary>
        ///     浏览器信息
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     操作系统
        /// </summary>
        public string OsVersion { get; set; }

        /// <summary>
        ///     登录时间
        /// </summary>
        public string LoginTime { get; set; }
        /// <summary>
        ///     创建人员
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        ///     创建人员登录代码
        /// </summary>
        public string CreateUserCode { get; set; }

        /// <summary>
        ///     创建人员名字
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        ///     登录类型:
        ///         0、账号密码登录
        ///         1、人脸识别登录
        /// </summary>
        public int LoginStatus{ get; set; }
    }
    /// <summary>
    ///     Sql日志
    /// </summary>
    public class SqlLog
    {
        /// <summary>
        ///     sql日志Id
        /// </summary>
        public Guid SqlLogId { get; set; }

        /// <summary>
        ///     操作sql
        /// </summary>
        public string OperateSql { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        public string EndDateTime { get; set; }

        /// <summary>
        ///     耗时
        /// </summary>
        public double ElapsedTime { get; set; }

        /// <summary>
        ///     参数
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        ///     创建人员
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        ///     创建人员登录代码
        /// </summary>
        public string CreateUserCode { get; set; }

        /// <summary>
        ///     创建人员名字
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
    /// <summary>
    ///     用户操作日志实体
    /// </summary>
    public class OperateLog
    {
        /// <summary>
        ///     主键编码
        /// </summary>
        public Guid OperateLogId { get; set; }
        /// <summary>
        ///     客户端
        /// </summary>
        public string ClientHost { get; set; }
        /// <summary>
        ///     服务端IP地址
        /// </summary>
        public string ServerHost { get; set; }
        /// <summary>
        ///     请求内容大小
        /// </summary>
        public int? RequestContentLength { get; set; }
        /// <summary>
        ///     请求类型get or post
        /// </summary>
        public string RequestType { get; set; }
        /// <summary>
        ///    当前请求Url信息
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///     上次请求Url信息
        /// </summary>
        public string UrlReferrer { get; set; }
        /// <summary>
        ///     请求数据
        /// </summary>
        public string RequestData { get; set; }
        /// <summary>
        ///     浏览器代理信息
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     控制器名称
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        ///     操作名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        ///     Action执行时间(秒)
        /// </summary>
        public double ActionExecutionTime { get; set; }

        /// <summary>
        ///     页面展示时间(秒)
        /// </summary>
        public double ResultExecutionTime { get; set; }

        /// <summary>
        ///     响应状态
        /// </summary>
        public string ResponseStatus { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        ///     创建人员
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        ///     创建人员登录代码
        /// </summary>
        public string CreateUserCode { get; set; }

        /// <summary>
        ///     创建人员名字
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
    /// <summary>
    ///     错误日志
    /// </summary>
    public class ExceptionLog {
        /// <summary>
        ///     主键编码
        /// </summary>
        public string ExceptionLogId { get; set; }
        /// <summary>
        ///     消息
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
        ///     请求方式
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        ///     创建人员
        /// </summary>
        public string CreateUserId { get; set; }
        public string CreateUserCode { get; set; }
        /// <summary>
        ///     创建人员姓名
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        ///     客户端地址
        /// </summary>
        public string ClientAddress { get; set; }
    }
}
