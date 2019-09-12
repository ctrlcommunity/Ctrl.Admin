using Ctrl.Plugin.Wx.Domain;
using Ctrl.Plugin.Wx.Response;

namespace Ctrl.Plugin.Wx.Request
{
    public class TransferRequest : BaseRequest<TransferModel, TransferResponse>
    {
        public TransferRequest()
        {
            RequestUrl = "/mmpaymkttransfers/promotion/transfers";
            IsUseCert = true;
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Add("mch_appid", merchant.AppId);
            GatewayData.Add("mchid", merchant.MchId);

            GatewayData.Remove("appid");
            GatewayData.Remove("mch_id");
            GatewayData.Remove("notify_url");
            GatewayData.Remove("sign_type");
        }
    }
}
