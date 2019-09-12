using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Ctrl.Core.Core.Http
{
    /// <summary>
    ///     Post/Get请求帮助类
    /// </summary>
    public static class RequestUtil
    {
        #region 通讯函数

        /// <summary>
        ///     通讯函数
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="para">请求参数</param>
        /// <param name="method">请求方式GET/POST</param>
        /// <returns></returns>
        public static string SendRequest(string url, string para, string method)
        {
            var strResult = "";
            if (string.IsNullOrEmpty(url))
                return null;
            if (string.IsNullOrEmpty(method))
                method = "GET";
            // GET方式
            if (method.ToUpper() == "GET")
            {
                try
                {
                    var wrq = WebRequest.Create(url + para);
                    wrq.Method = "GET";
                    // ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    var wrp = wrq.GetResponse();
                    var sr = new StreamReader(wrp.GetResponseStream(), Encoding.GetEncoding("gb2312"));
                    strResult = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            // POST方式
            if (method.ToUpper() == "POST")
            {
                if (para.Length > 0 && para.IndexOf('?') == 0)
                {
                    para = para.Substring(1);
                }
                var req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                var urlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                {
                    int i = 0, j;
                    while (i < para.Length)
                    {
                        j = para.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            urlEncoded.Append(HttpUtility.UrlEncode(para.Substring(i, para.Length - i),
                                Encoding.GetEncoding("utf-8")));
                            break;
                        }
                        urlEncoded.Append(HttpUtility.UrlEncode(para.Substring(i, j - i), Encoding.GetEncoding("utf-8")));
                        urlEncoded.Append(para.Substring(j, 1));
                        i = j + 1;
                    }
                    var someBytes = Encoding.Default.GetBytes(urlEncoded.ToString());
                    req.ContentLength = someBytes.Length;
                    var newStream = req.GetRequestStream();
                    newStream.Write(someBytes, 0, someBytes.Length);
                    newStream.Close();
                }
                try
                {
                    var result = req.GetResponse();
                    var receiveStream = result.GetResponseStream();
                    var read = new Byte[512];
                    if (receiveStream != null)
                    {
                        var bytes = receiveStream.Read(read, 0, 512);
                        while (bytes > 0)
                        {
                            // 注意：
                            // 下面假定响应使用 UTF-8 作为编码方式。
                            // 如果内容以 ANSI 代码页形式（例如，932）发送，则使用类似下面的语句：
                            // Encoding encode = System.Text.Encoding.GetEncoding("shift-jis");
                            var encode = Encoding.GetEncoding("utf-8");
                            strResult += encode.GetString(read, 0, bytes);
                            bytes = receiveStream.Read(read, 0, 512);
                        }
                    }
                    return strResult;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return strResult;
        }

        #endregion

        #region 简化通讯函数

        /// <summary>
        ///     GET方式通讯
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string SendGetRequest(string url,
            string para)
        {
            return SendRequest(url, para, "GET");
        }

        /// <summary>
        ///     POST方式通讯
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string SendPostRequest(string url,
            string para)
        {
            return SendRequest(url, para, "POST");
        }

        #endregion
    }
}
