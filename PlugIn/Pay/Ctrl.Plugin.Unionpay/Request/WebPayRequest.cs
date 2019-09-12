using Ctrl.Plugin.Unionpay.Domain;
using Ctrl.Plugin.Unionpay.Response;

namespace Ctrl.Plugin.Unionpay.Request
{
    public class WebPayRequest : BaseRequest<WebPayModel, WebPayResponse>
    {
        public WebPayRequest()
        {
            RequestUrl = "/gateway/api/frontTransReq.do";
        }
    }
}
