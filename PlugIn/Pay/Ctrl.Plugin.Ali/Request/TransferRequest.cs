using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Response;

namespace Ctrl.Plugin.Ali.Request
{
    public class TransferRequest : BaseRequest<TransferModel, TransferResponse>
    {
        public TransferRequest()
            : base("alipay.fund.trans.toaccount.transfer")
        {
        }
    }
}
