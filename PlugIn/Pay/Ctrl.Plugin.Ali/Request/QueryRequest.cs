using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Response;

namespace Ctrl.Plugin.Ali.Request
{
    public class QueryRequest : BaseRequest<QueryModel, QueryResponse>
    {
        public QueryRequest()
            : base("alipay.trade.query")
        {
        }
    }
}
