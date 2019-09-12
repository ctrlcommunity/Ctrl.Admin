using Ctrl.Plugin.Unionpay.Domain;
using Ctrl.Plugin.Unionpay.Response;

namespace Ctrl.Plugin.Unionpay.Request
{
    public class ScanPayRequest : BaseRequest<ScanPayModel, ScanPayResponse>
    {
        public ScanPayRequest()
        {
            RequestUrl = "/gateway/api/backTransReq.do";
        }
    }
}
