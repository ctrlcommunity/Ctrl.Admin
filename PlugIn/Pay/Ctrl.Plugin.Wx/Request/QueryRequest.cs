using Ctrl.Plugin.Wx.Domain;
using Ctrl.Plugin.Wx.Response;

namespace Ctrl.Plugin.Wx.Request
{
    public class QueryRequest : BaseRequest<QueryModel, QueryResponse>
    {
        public QueryRequest()
        {
            RequestUrl = "/pay/orderquery";
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("notify_url");
        }
    }
}
