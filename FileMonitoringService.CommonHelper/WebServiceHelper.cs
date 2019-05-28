using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringService.CommonHelper
{
    public class WebServiceHelper
    { 
        /// <summary>  
        /// GET请求与获取结果  
        /// </summary>  
        public static string HttpGet(string Url, string postDataStr)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            //request.Method = "GET";
            //request
            //request.ContentType = "application/json";//Content-Type:application/json

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream myResponseStream = response.GetResponseStream();
            //StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            //string retString = myStreamReader.ReadToEnd();
            //myStreamReader.Close();
            //myResponseStream.Close();

            //return retString;
            var request = (HttpWebRequest)WebRequest.Create(Url);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString.ToString();
        }

        /// <summary>  
        /// 指定Post地址使用Get 方式获取全部字符串  
        /// </summary>  
        /// <param name="url">请求后台地址</param>  
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>  
        /// <returns></returns>  
        public static string Post(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";// "application/x-www-form-urlencoded";

            #region 添加Post 参数
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }  
    }
}
