using Ctrl.Core.Core.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace Ctrl.Core.Core.Web
{
    /// <summary>
    ///     Ip访问助手
    /// </summary>
    public static class IpBrowserUtil
    {

        #region 获取客户端Ip

        /// <summary>
        ///     获取客户端Ip
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            try
            {
                var ip = IPAddress.Loopback.ToString();

                ip = HttpContexts.Current.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }
                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContexts.Current.Connection.RemoteIpAddress.ToString();
                }
                return ip;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region 获取客户端主机名

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetClientHost()
        {
            try
            {
                var address = IPAddress.Parse(GetClientIp());
                var ipInfor = Dns.GetHostEntry(address);
                return ipInfor.HostName;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region GetLocalIPs

        public static HashSet<string> GetLocalIPs()
        {
            var ips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var strPcName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(strPcName);
            foreach (var ip in ipEntry.AddressList.Where(ip => IsRightIp(ip.ToString())))
            {
                ips.Add(ip.ToString());
            }

            return ips;
        }

        #endregion

        #region 获取服务器计算机主机名

        /// <summary>
        ///     获取本地计算机主机名
        /// </summary>
        /// <returns></returns>
        public static string GetServerHost()
        {
            try
            {
                return Dns.GetHostName();
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region 获取服务器端相关的Ip地址

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetServerHostIp()
        {
            try
            {
                return HttpContexts.Current.Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            }
            catch
            {
   
                return "";
            }
        }

        #endregion

        #region 得到网关地址

        /// <summary>
        ///     得到网关地址
        /// </summary>
        /// <returns></returns>
        public static string GetGateway()
        {
            var strGateway = "";
            //获取所有网卡
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (
                var gateways in
                    nics.Select(netWork => netWork.GetIPProperties()).Select(ip => ip.GatewayAddresses))
            {
                foreach (
                    var gateWay in
                        gateways.Where(gateWay => IsPingIp(gateWay.Address.ToString())))
                {
                    strGateway = gateWay.Address.ToString();
                    break;
                }

                if (strGateway.Length > 0)
                {
                    break;
                }
            }

            return strGateway;
        }

        #endregion

        #region 判断是否为正确的IP地址

        /// <summary>
        ///     判断是否为正确的IP地址
        /// </summary>
        /// <param name="strIPadd">需要判断的字符串</param>
        /// <returns>true = 是 false = 否</returns>
        public static bool IsRightIp(string strIPadd)
        {
            if (Regex.IsMatch(strIPadd, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                //根据小数点分拆字符串
                var ips = strIPadd.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    return Int32.Parse(ips[0]) < 256 &&
                           Int32.Parse(ips[1]) < 256 & Int32.Parse(ips[2]) < 256 & Int32.Parse(ips[3]) < 256;
                }
                return false;
            }
            return false;
        }

        #endregion

        #region 尝试Ping指定IP是否能够Ping通

        /// <summary>
        ///     尝试Ping指定IP是否能够Ping通
        /// </summary>
        /// <param name="strIp">指定IP</param>
        /// <returns>true 是 false 否</returns>
        public static bool IsPingIp(string strIp)
        {
            try
            {
                var ping = new Ping();
                ping.Send(strIp, 1000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 得到本机IP

        /// <summary>
        ///     得到本机IP
        /// </summary>
        public static string GetLocalIp()
        {
            var strLocalIp = "";
            var strPcName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(strPcName);
            foreach (var ip in ipEntry.AddressList.Where(ip => IsRightIp(ip.ToString())))
            {
                strLocalIp = ip.ToString();
                break;
            }

            return strLocalIp;
        }

        #endregion

        #region 获取浏览器信息

        /// <summary>
        ///     获取浏览器版本号
        /// </summary>
        /// <returns></returns>
        public static string GetBrowser()
        {
            var bc = HttpContexts.Current.Request.Headers["user-agent"].FirstOrDefault();
            return bc;
        }

        #endregion

        #region 获取操作系统版本号

        /// <summary>
        ///     获取操作系统版本号
        /// </summary>
        /// <returns></returns>
        public static string GetOsVersion()
        {
            //UserAgent 
            var userAgent = HttpContexts.Current.Request.Headers["user-agent"].FirstOrDefault();

            var osVersion = "未知";

            if (userAgent.Contains("NT 10.0"))
            {
                osVersion = "Windows 10";
            }
            else if (userAgent.Contains("NT 6.3"))
            {
                osVersion = "Windows 8.1";
            }
            else if (userAgent.Contains("NT 6.2"))
            {
                osVersion = "Windows 8";
            }
            else if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                if (userAgent.Contains("64"))
                    osVersion = "Windows XP";
                else
                    osVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            else
            {
                osVersion = HttpContexts.Current.Request.Headers["user-agent"].FirstOrDefault();
            }
            return osVersion;
        }

        #endregion

        #region 获取客户端IP地址

        /// <summary>
        ///     获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            var result = HttpContexts.Current.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContexts.Current.Request.Headers["REMOTE_ADDR"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(result))
            {
                return "0.0.0.0";
            }
            return result;
        }

        #endregion

        #region  判断是否是IP格式

        /// <summary>
        ///     判断是否是IP地址格式  0.0.0.0
        /// </summary>
        /// <param name="str1">待判断的IP地址</param>
        /// <returns>true  or  false</returns>
        public static bool IsIpAddress(string str1)
        {
            if (string.IsNullOrEmpty(str1) || str1.Length < 7 || str1.Length > 15) return false;

            const string regFormat = @"^d{1,3}[.]d{1,3}[.]d{1,3}[.]d{1,3}$";

            var regex = new Regex(regFormat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }

        #endregion

        #region 获取公网IP

        /// <summary>
        ///     获取公网IP
        /// </summary>
        /// <returns></returns>
        public static string GetNetIp()
        {
            var tempIp = "";
            try
            {
                var wr = WebRequest.Create("http://city.ip138.com/ip2city.asp");
                var s = wr.GetResponse().GetResponseStream();
                if (s != null)
                {
                    var sr = new StreamReader(s, Encoding.GetEncoding("gb2312"));
                    var all = sr.ReadToEnd(); //读取网站的数据

                    var start = all.IndexOf("[", StringComparison.Ordinal) + 1;
                    var end = all.IndexOf("]", start, StringComparison.Ordinal);
                    tempIp = all.Substring(start, end - start);
                    sr.Close();
                }
                if (s != null) s.Close();
            }
            catch
            {
                if (Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length > 1)
                    tempIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                if (string.IsNullOrEmpty(tempIp))
                    return GetIp();
            }
            return tempIp;
        }

        #endregion

        #region 获取物理地址

        /// <summary>
        ///     根据提供的api获取物理地址
        /// </summary>
        /// <returns></returns>
        public static string GetAddressByApi()
        {
            try
            {
                var apiUrl = "http://whois.pconline.com.cn/ip.jsp?";
                var ip = String.Format("ip={0}", GetClientIp());
                return RequestUtil.SendGetRequest(apiUrl, ip).Replace("\n", string.Empty).Replace("\r", string.Empty);
            }
            catch 
            {
                return "";
            }

        }

        #endregion
    }
}
