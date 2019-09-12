using Ctrl.Plugin.PayCore.Request;
using Ctrl.Plugin.PayCore.Response;

namespace Ctrl.Plugin.PayCore.Gateways
{
    public interface IGateway
    {
        TResponse Execute<TModel, TResponse>(Request<TModel, TResponse> request) where TResponse : IResponse;
    }
}
