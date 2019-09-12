using Ctrl.Plugin.PayCore.Request;

namespace Ctrl.Plugin.Wx.Response
{
    public class CloseResponse : BaseResponse
    {
        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}
