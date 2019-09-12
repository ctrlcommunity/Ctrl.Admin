using Ctrl.Plugin.PayCore.Request;

namespace Ctrl.Plugin.Ali.Response
{
    public class CloseResponse : BaseResponse
    {
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}
