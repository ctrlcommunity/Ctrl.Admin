﻿using Ctrl.Plugin.Wx.Domain;
using Ctrl.Plugin.Wx.Response;

namespace Ctrl.Plugin.Wx.Request
{
    public class TransferQueryRequest : BaseRequest<TransferQueryModel, TransferQueryResponse>
    {
        public TransferQueryRequest()
        {
            RequestUrl = "/mmpaymkttransfers/gettransferinfo";
            IsUseCert = true;
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("notify_url");
            GatewayData.Remove("sign_type");
        }
    }
}
