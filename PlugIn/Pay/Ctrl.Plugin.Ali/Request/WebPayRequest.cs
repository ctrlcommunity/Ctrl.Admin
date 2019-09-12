using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Response;

namespace Ctrl.Plugin.Ali.Request
{
    public class WebPayRequest : BaseRequest<WebPayModel, WebPayResponse>
    {
        public WebPayRequest()
            : base("alipay.trade.page.pay")
        {
        }
    }
}
