using Ctrl.Plugin.PayCore.Gateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Plugin.PayCore.Events
{
    public class UnKnownNotifyEventArgs : NotifyEventArgs
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gateway">支付网关</param>
        public UnKnownNotifyEventArgs(BaseGateway gateway)
            : base(gateway)
        {
        }

        #endregion

        #region 属性

        public string Message { get; set; }

        #endregion
    }
}
