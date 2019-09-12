using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Response;

namespace Ctrl.Plugin.Ali.Request
{
    public class ScanPayRequest : BaseRequest<ScanPayModel, ScanPayResponse>
    {
        public ScanPayRequest()
            : base("alipay.trade.precreate")
        {
        }
    }
}
