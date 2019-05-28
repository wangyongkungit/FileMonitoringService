using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringService.CommonHelper
{
    public static class DateTimeUtility
    {
        /// <summary>
        /// 将指定的 C# 时间转为 JavaScript 时间戳（格林威治时间1970年01月01日00时00分00秒(北京时间1970年01月01日08时00分00秒)起至现在的总毫秒数）
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long CSharpTimeToJavaScriptTime(DateTime datetime)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(datetime - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }
    }
}
