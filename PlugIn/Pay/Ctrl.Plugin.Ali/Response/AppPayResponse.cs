using Ctrl.Plugin.Ali.Request;
using Ctrl.Plugin.PayCore.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Plugin.Ali.Response
{
    public class AppPayResponse : IResponse
    {
        public AppPayResponse(AppPayRequest request)
        {
            OrderInfo = request.GatewayData.ToUrl();
        }

        /// <summary>
        /// 用于唤起App的订单参数
        /// </summary>
        public string OrderInfo { get; set; }

        public string Raw { get; set; }
    }
}
