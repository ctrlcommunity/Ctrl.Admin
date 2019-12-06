using NLog;

namespace Ctrl.Core.Core.Log
{
    /// <summary>
    ///     说明：日志记录基类
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    public abstract class BaseHandler<TLog>
    {
        #region 构造函数
        /// <summary>
        ///     构造函数初始化
        /// </summary>
        /// <param name="loggerConfig"></param>
        protected BaseHandler(string loggerConfig) {
            LoggerConfig = loggerConfig;
        }
        #endregion

        #region 方法
        /// <summary>
        ///     写入日志，虚函数，可进行重写
        /// </summary>
        public virtual void WriteLog() {
            if (string.IsNullOrEmpty(LoggerConfig))
            {
                return;
            }
            Logger iLog = LogManager.GetLogger(LoggerConfig);
            if (iLog.IsInfoEnabled)
            {
            
                iLog.Info(log);
            }
        }

        #endregion



        #region 属性
        /// <summary>
        ///     需要启动的日志模式名称
        /// </summary>
        private string LoggerConfig { get; set; }

        public TLog log { get; set; }
        #endregion
    }
}
