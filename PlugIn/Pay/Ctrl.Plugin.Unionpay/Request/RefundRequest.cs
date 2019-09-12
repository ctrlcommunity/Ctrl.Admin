using Ctrl.Plugin.Unionpay.Domain;
using Ctrl.Plugin.Unionpay.Response;

namespace Ctrl.Plugin.Unionpay.Request
{
    public class RefundRequest : BaseRequest<RefundModel, RefundResponse>
    {
        public RefundRequest()
        {
            RequestUrl = "/gateway/api/backTransReq.do";
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("frontUrl");
        }
    }
}
