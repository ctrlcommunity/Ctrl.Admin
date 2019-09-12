using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Plugin.Wx.Domain
{
    public class OAuthModel
    {
        /// <summary>
        /// 微信Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        public string GrantType => "authorization_code";
    }
}
