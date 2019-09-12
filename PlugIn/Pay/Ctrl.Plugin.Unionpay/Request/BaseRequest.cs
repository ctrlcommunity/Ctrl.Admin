using Ctrl.Plugin.PayCore.Request;
using Ctrl.Plugin.PayCore.Response;
using Ctrl.Plugin.PayCore.Utils;
using System;

namespace Ctrl.Plugin.Unionpay.Request
{
    public class BaseRequest<TModel, TResponse> : Request<TModel, TResponse> where TResponse : IResponse
    {
        public BaseRequest()
            : base(StringComparer.Ordinal)
        {
        }

        public override void AddGatewayData(TModel model)
        {
            base.AddGatewayData(model);

            GatewayData.Add(model, StringCase.Camel);
        }

        internal virtual void Execute(Merchant merchant)
        {
        }
    }
}
