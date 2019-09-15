#if NETCOREAPP
using Baidu.Aip.Face;
using Ctrl.Core.Core.Config;
using Ctrl.Core.Core.Converts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ctrl.Core.Core.BaiduFace
{
    /// <summary>
    ///    <see cref="Baidu.Aip.Face"/> 人脸识别帮助类
    /// </summary>
    public class FaceUtil : IDisposable
    {

        public static string APPID = AppSetting.Load().BaiDuStrings.FirstOrDefault().APPID;
        public static string APIKEY = AppSetting.Load().BaiDuStrings.FirstOrDefault().APIKEY;
        public static string SECRETKEY = AppSetting.Load().BaiDuStrings.FirstOrDefault().SECRETKEY;
        private Face client { get; set; }
        public FaceUtil()
        {
            client = new Face(APIKEY, SECRETKEY);
            client.Timeout = 60000;  // 修改超时时间
        }
        /// <summary>
        ///     新增保存用户脸部
        /// </summary>
        /// <returns></returns>
        public FaceSearchInfo UserFaceSave(string base64, string userid)
        {
            try
            {
                var image = base64;
                var imageType = "BASE64";
                var groupId = "group1";
                var userId = userid.Replace('-','M');
                var result = client.UserAdd(image, imageType, groupId, userId);
                return result.ToObject<FaceSearchInfo>();
            }
            catch
            {
                throw;
            }

        }
        public FaceSearchInfo SearchFace(string facebase64) {
            try
            {
                FaceSearchInfo faceSearchInfo = new FaceSearchInfo();
                var image = facebase64;
                var imageType = "BASE64";
                var groupIdList = "group1";
                var result = client.Search(image, imageType, groupIdList);
                faceSearchInfo= result.ToObject<FaceSearchInfo>();
                faceSearchInfo.result.user_id = result["result"]["user_list"].FirstOrDefault().Value<string>("user_id");
                return faceSearchInfo;
            }
            catch 
            {
                return null;
            }
         
        }
        public void Dispose()
        {
            if (client != null)
            {
                try
                {
                    client = null;
                }
                catch
                {
                }
            }
        }
    }
}
#endif