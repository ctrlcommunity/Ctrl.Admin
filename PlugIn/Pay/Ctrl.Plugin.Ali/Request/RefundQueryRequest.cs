using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Plugin.Ali.Request
{
    public class RefundQueryRequest : BaseRequest<RefundQueryModel, RefundQueryResponse>
    {
        public RefundQueryRequest()
            : base("alipay.trade.fastpay.refund.query")
        {
        }
    }
}
