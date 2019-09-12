using Ctrl.Plugin.PayCore;
using Ctrl.Plugin.PayCore.Request;

namespace Ctrl.Plugin.Wx.Response
{
    public class PublicKeyResponse : BaseResponse
    {
        /// <summary>
        /// RSA 公钥
        /// </summary>
        [ReName(Name = "pub_key")]
        public string PublicKey { get; set; }

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}
