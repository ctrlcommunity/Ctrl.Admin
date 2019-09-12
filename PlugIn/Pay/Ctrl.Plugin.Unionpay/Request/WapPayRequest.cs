using Ctrl.Plugin.Unionpay.Domain;
using Ctrl.Plugin.Unionpay.Response;

namespace Ctrl.Plugin.Unionpay.Request
{
    public class WapPayRequest : BaseRequest<WapPayModel, WapPayResponse>
    {
        public WapPayRequest()
        {
            RequestUrl = "/gateway/api/frontTransReq.do";
        }
    }
}
