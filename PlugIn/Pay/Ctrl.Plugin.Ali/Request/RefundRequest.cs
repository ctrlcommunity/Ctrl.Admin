using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Response;

namespace Ctrl.Plugin.Ali.Request
{
    public class RefundRequest : BaseRequest<RefundModel, RefundResponse>
    {
        public RefundRequest()
            : base("alipay.trade.refund")
        {
        }
    }
}
