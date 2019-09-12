using Ctrl.Core.Core.Utils;
using Ctrl.Core.Core.Web;
using System;

namespace Ctrl.Core.Core.Log
{
    /// <summary>
    ///     构造函数
    /// </summary>
    public class LoginLogHandler:BaseHandler<LoginLog>
    {
        public LoginLogHandler(string UserId,string Code,string UserName,int LoginStatus):base("LoginLogToDatabase") {
            log = 
            new LoginLog
            {
                LoginLogId = CombUtil.NewComb().ToString(),
                CreateUserId = UserId,
                CreateUserCode = Code ?? "",
                ServerHost = String.Format("{0}【{1}】", IpBrowserUtil.GetServerHost(), IpBrowserUtil.GetServerHostIp()),
                ClientHost = String.Format("{0}", IpBrowserUtil.GetClientIp()),
                UserAgent = IpBrowserUtil.GetBrowser(),
                OsVersion = IpBrowserUtil.GetOsVersion(),
                LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IpAddressName = IpBrowserUtil.GetAddressByApi(),
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateUserName = UserName,
                LoginStatus=LoginStatus

            };
        }
    }
}
