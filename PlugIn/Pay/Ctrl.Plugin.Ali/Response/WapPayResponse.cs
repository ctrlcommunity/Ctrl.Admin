using Ctrl.Plugin.Ali.Request;
using Ctrl.Plugin.PayCore.Response;

namespace Ctrl.Plugin.Ali.Response
{
    public class WapPayResponse : IResponse
    {
        public WapPayResponse(WapPayRequest request)
        {
            Url = $"{request.RequestUrl}&{request.GatewayData.ToUrl()}";
        }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Url { get; set; }

        public string Raw { get; set; }
    }
}
