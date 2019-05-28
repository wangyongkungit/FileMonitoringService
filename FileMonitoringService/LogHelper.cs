using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoring
{
    public class LogHelper
    {
        static string logFile = string.Format("{0}/{1}/{2}.txt", System.AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["logPath"], System.DateTime.Now.ToString("yyyy-MM-dd"));
        /// <summary>
        /// 不带参数的构造函数
        /// </summary>
        public LogHelper()
        {
            logFile = string.Format("{0}/{1}/{2}.txt", System.AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["logPath"], System.DateTime.Now.ToString("yyyy-MM-dd"));
        }
        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="logFile"></param>
        public LogHelper(string logFile)
        {
            LogHelper.logFile = logFile;
        }
        /// <summary>
        /// 追加一条信息
        /// </summary>
        /// <param name="text"></param>
        public static void Write(string text)
        {
            using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
            {
                sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
            }
        }
        /// <summary>
        /// 追加一条信息
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="text"></param>
        public static void Write(string logFile, string text)
        {
            using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
            {
                sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
            }
        }
        /// <summary>
        /// 追加一行信息
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLine(string text)
        {
            text += "\r\n";
            logFile = string.Format("{0}/{1}/{2}.txt", System.AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["logPath"], System.DateTime.Now.ToString("yyyy-MM-dd"));
            using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
            {
                sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
            }
        }
        /// <summary>
        /// 追加一行信息
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="text"></param>
        public static void WriteLine(string logFile, string text)
        {
            text += "\r\n";
            using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
            {
                sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
            }
        }
    }
}
