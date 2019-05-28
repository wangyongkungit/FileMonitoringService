using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileMonitoringService.CommonHelper
{
    public static class DingTalkHelper
    {
        public static string GetAccessToken()
        {
            string apiUrl = "https://oapi.dingtalk.com/gettoken?corpid=ding08a708c5272bc85d35c2f4657eb6378f&corpsecret=o4Ivoh4T7MfhOGf2wlIZmzrUih03dDw2OcvekuZOGUohFj-CvlyOej2DZHRx_-By";
            string result = WebServiceHelper.HttpGet(apiUrl, null);
            JObject jObj = JObject.Parse(result);
            //返回码
            string errcode = jObj["errcode"].ToString();
            //accessToken
            string accessToken = string.Empty;
            //获取成功
            if (errcode == "0")
            {
                accessToken = jObj["access_token"].ToString();
            }
            return accessToken;
        }

        public static string UrlEncodeNew(string originalStr)
        {
            if (!string.IsNullOrEmpty(originalStr))
            {
                return HttpUtility.UrlEncode(originalStr).Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
            }
            return "";
        }
    }
}
