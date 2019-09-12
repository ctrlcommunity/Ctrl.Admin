using Ctrl.Plugin.Wx.Domain;
using Ctrl.Plugin.Wx.Response;

namespace Ctrl.Plugin.Wx.Request
{
    public class RefundRequest : BaseRequest<RefundModel, RefundResponse>
    {
        public RefundRequest()
        {
            RequestUrl = "/secapi/pay/refund";
            IsUseCert = true;
        }
    }
}
