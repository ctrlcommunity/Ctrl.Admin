using Ctrl.Plugin.PayCore.Response;
using Ctrl.Plugin.Unionpay.Request;

namespace Ctrl.Plugin.Unionpay.Response
{
    public class WapPayResponse : IResponse
    {
        public WapPayResponse(WapPayRequest request)
        {
            Html = request.GatewayData.ToForm(request.RequestUrl);
        }

        /// <summary>
        /// 生成的Html网页
        /// </summary>
        public string Html { get; set; }

        public string Raw { get; set; }
    }
}
