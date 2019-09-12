using Ctrl.Plugin.PayCore.Request;

namespace Ctrl.Plugin.Unionpay.Response
{
    public class ScanPayResponse : BaseResponse
    {
        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode { get; set; }

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}
