using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FileMonitoringServiceBLL;

namespace FileMonitoring
{
    public partial class FileMonitoringService : ServiceBase
    {
        public FileMonitoringService()
        {
            InitializeComponent();
        }

        System.Timers.Timer timer;
        TaskRemindingBLL trBll = new TaskRemindingBLL();

        /// <summary>
        /// 服务启动事件
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            LogHelper.WriteLine("服务启动");
            timer = new System.Timers.Timer();
            //间隔
            int interval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"]) * 1000;
            timer.Interval = interval;
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //WriteFileMonitoring();
            TaskMonitoring();
            SetWorkingSet(750000);
        }

        /// <summary>
        /// 任务监控（新）
        /// </summary>
        private void TaskMonitoring()
        {
            try
            {
                string empNumberEnable = Convert.ToString(ConfigurationManager.AppSettings["empNumberEnable"]);//状态为启用的员工集合
                string empFolderPath = Convert.ToString(ConfigurationManager.AppSettings["employeePath"]);  //员工目录                
                string modifyRecordFolder = ConfigurationManager.AppSettings["modifyRecordFolderName"].ToString();//存放修改记录的文件夹名
                DirectoryInfo empRootFolder = new DirectoryInfo(empFolderPath);
                ProjectBLL pBll = new ProjectBLL();
                DataTable dtProject = pBll.GetProject();
                if (dtProject != null)
                {
                    //遍历“员工”目录下的所有文件夹，即这个循环将获得每个员工的文件夹
                    //LogHelper.WriteLine("最大的遍历之前");
                    foreach (DirectoryInfo empFolder in empRootFolder.GetDirectories())
                    {
                        //判断是启用的员工
                        if (empNumberEnable.Contains(empFolder.Name))
                        {
                            //LogHelper.WriteLine("判断是启用的员工empFolder.Name" + empFolder.Name);
                            //文件夹名即是员工编号
                            string empNo = empFolder.Name;

                            //循环每个员工下的任务，此循环将得到各个员工文件夹下每个任务的文件夹
                            foreach (DirectoryInfo taskFolder in empFolder.GetDirectories())
                            {
                                //LogHelper.WriteLine("循环每个员工下的任务taskFolder.Name：" + taskFolder.Name);
                                //循环数据库中已创建的任务列表，这个循环用来匹配并设置完成人
                                for (int i = 0; i < dtProject.Rows.Count; i++)
                                {
                                    string projectID = dtProject.Rows[i]["ID"].ToString();
                                    string taskNO = dtProject.Rows[i]["TASKNO"].ToString();
                                    //如果数据库中记录的任务名跟当前目录相符
                                    if (taskFolder.Name.StartsWith(taskNO))
                                    {
                                        LogHelper.WriteLine("如果数据库中记录的任务名跟当前目录相符taskNO：" + taskNO);
                                        //下面首先判断该任务是否已经设置了完成人
                                        string outUserNo=string.Empty;
                                        bool isSetFnsPerson = pBll.IsSetFinishedPerson(projectID, out outUserNo);
                                        LogHelper.WriteLine("outUserNo:" + outUserNo+"  isSetFnsPerson:"+isSetFnsPerson);
                                        if (isSetFnsPerson)
                                        {
                                            //如果设置了完成人，再对两次完成人是否一致进行比较，不一致的情况下才更新
                                            if (empNo != outUserNo)
                                            {
                                                //设置完成人
                                                bool updateFlag = pBll.UpdateFinishedPerson(empNo, projectID);
                                                //LogHelper.WriteLine("updateFlag：" + updateFlag);
                                                if (updateFlag)
                                                {
                                                    LogHelper.WriteLine(string.Format("设置了任务[{0}]的完成人为[{1}]", taskNO, empNo));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //设置完成人
                                            bool updateFlag = pBll.UpdateFinishedPerson(empNo, projectID);
                                            if (updateFlag)
                                            {
                                                LogHelper.WriteLine(string.Format("设置了任务[{0}]的完成人为[{1}]", taskNO, empNo));
                                            }
                                        }
                                        //如果文件夹名称中包含“完成”，则说明任务已完成
                                        if (taskFolder.Name.Contains("完成"))
                                        {
                                            //LogHelper.WriteLine("如果文件夹名称中包含“完成”，则说明任务已完成");
                                            bool flagUptFinish = pBll.UpdateFinishedStatus(projectID);
                                        }
                                    }
                                }
                                //遍历“修改记录”文件夹，这个循环用来检测修改记录的完成状态
                                foreach (DirectoryInfo taskFolderChild in taskFolder.GetDirectories())
                                {
                                    LogHelper.WriteLine("因为有一些命名不规范的目录名，故要进行判断。taskFolder.Name：" + taskFolder.Name);
                                    LogHelper.WriteLine("因为有一些 taskFolderChild.Name：" + taskFolderChild.Name);
                                    //因为有一些命名不规范的目录名，故要进行判断
                                    if (taskFolder.Name.Length >= 22)
                                    {
                                        //如果遍历到是修改记录文件夹
                                        LogHelper.WriteLine("如果遍历到是修改记录文件夹.taskFolderChild.Name:" + taskFolderChild.Name);
                                        if (taskFolderChild.Name.Trim() == modifyRecordFolder)
                                        {
                                            LogHelper.WriteLine("taskFolderChild.Name.Trim() == modifyRecordFolder");
                                            foreach (DirectoryInfo modifyFolders in taskFolderChild.GetDirectories())
                                            {
                                                try
                                                {
                                                    //获取任务目录的原始文件名（因为任务文件夹名称会发生变动，因此取前26个不会变动的字符）
                                                    LogHelper.WriteLine("taskFolder.Name.Substring(0, 26);");
                                                    string taskNoOriginal = taskFolder.Name.Substring(0, 22);
                                                    //修改记录文件夹（原始名）
                                                    string modfItem = modifyFolders.Name.Trim();
                                                    //LogHelper.WriteLine("modfItem：" + modfItem);
                                                    #region Remarks 2017-03-11 14:02:54
                                                    ////修改记录文件夹（完成后的，会加上“完成”二字）
                                                    //string modfItemOriginal = modfItem.Replace("完成", string.Empty);
                                                    ////判断数据库中是否存在该修改记录
                                                    //bool isExistModify = pBll.IsExistProjectModify(taskNoOriginal, modfItemOriginal);
                                                    ////不存在则添加
                                                    ////LogHelper.WriteLine("isExistMofify：" + isExistMofify);
                                                    //if (!isExistModify)
                                                    //{
                                                    //    //首先判断这个目录对应的任务在数据库中存不存在（否则有一些不符合命名规范或者未通过系统录入的任务，就会在ProjectModify表中生成冗余的数据）
                                                    //    bool IsExistProject = pBll.IsExistProjectByTaskNo(taskNoOriginal);
                                                    //    //存在再添加
                                                    //    if (IsExistProject)
                                                    //    {
                                                    //        bool flagAddProjectModify = pBll.AddProjectModify(taskNoOriginal, modfItemOriginal);
                                                    //    }
                                                    //}
                                                    ////存在的话判断是否完成，若已完成则将完成状态置为1（即已完成）
                                                    //else if (modfItem.Contains("完成"))
                                                    //{
                                                    //    bool flagModifyFinished = pBll.UpdateProjectModifyFinished(taskNoOriginal, modfItemOriginal);
                                                    //}
                                                    #endregion

                                                    /*2017-03-11修改，因需求变更，原稿和完成稿均需记录在数据库中*/
                                                    string projectModifyID = string.Empty;
                                                    string prjMdfFolder = string.Empty;
                                                    int reviewStatus = 0;
                                                    bool isExistModify1 = pBll.IsExistModifyTask(taskNoOriginal, out projectModifyID, out prjMdfFolder, out reviewStatus);
                                                    LogHelper.WriteLine("isExistModify1:" + isExistModify1);
                                                    if (isExistModify1 && reviewStatus == 0)
                                                    {
                                                        bool isSuccessSet = pBll.SetReviewPass(projectModifyID);
                                                        if (isSuccessSet)
                                                            LogHelper.WriteLine(string.Format("成功设置了任务ID为{0}的修改任务审核状态", projectModifyID));
                                                        else
                                                            LogHelper.WriteLine(string.Format("设置任务ID为{0}的修改任务审核状态失败", projectModifyID));
                                                    }
                                                    //如果目录名包含“完成”二字并且是以“修改n”打头的，则说明该修改任务已完成，将其录入数据库
                                                    LogHelper.WriteLine("modifyFolders.Name:" + modifyFolders.Name + "    modfItem:" + modfItem + "   prjMdfFolder:" + prjMdfFolder);
                                                    if (modifyFolders.Name.Contains("完成") && /*modifyFolders.Name*/ modfItem.StartsWith(prjMdfFolder))
                                                    {
                                                        LogHelper.WriteLine("modifyFolders.Name.Contains(\"完成\") && modifyFolders.Name == modfItem.Replace(\"完成\", string.Empty)");
                                                        DataRow[] dr = dtProject.Select(string.Format(" taskNO = '{0}'", taskNoOriginal));
                                                        LogHelper.WriteLine("DataRow[] dr的长度：" + dr.Length);
                                                        string projectID = string.Empty;
                                                        if (dr.Length > 0)
                                                        {
                                                            projectID = dr[0]["ID"].ToString();
                                                        }
                                                        bool isExistFinalModifyScript = pBll.IsExistFinalModifyScript(projectID, modifyFolders.Name);
                                                        LogHelper.WriteLine("isExistFinalModifyScript:" + isExistFinalModifyScript + "   modifyFolders.Name:" + modifyFolders.Name);
                                                        if (!isExistFinalModifyScript)
                                                        {
                                                            bool addFlag = pBll.AddProjectModify(projectID, modifyFolders.Name, 1, 1, DateTime.Now);
                                                            if (addFlag)
                                                            {
                                                                LogHelper.WriteLine(string.Format("[Success]projectID为[{0}]任务的修改完成稿[{1}]创建成功", projectID, modifyFolders.Name));
                                                            }
                                                            else
                                                            {
                                                                LogHelper.WriteLine(string.Format("[Error]projectID为[{0}]任务的修改完成稿[{1}]创建失败", projectID, modifyFolders.Name));
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LogHelper.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 写入记录到数据库
        /// </summary>
        protected void WriteFileMonitoring()
        {
            try
            {
                string empNosEnable = Convert.ToString(ConfigurationManager.AppSettings["empNumberEnable"]);    //状态为启用的员工集合
                string empFolderPath = Convert.ToString(ConfigurationManager.AppSettings["employeePath"]);  //员工目录
                DirectoryInfo empRootFolder = new DirectoryInfo(empFolderPath);
                //遍历整个员工的目录
                foreach (DirectoryInfo empFolder in empRootFolder.GetDirectories())
                {
                    if (empNosEnable.Contains(empFolder.Name))
                    {
                        Dictionary<string, string> dicFolder = new Dictionary<string, string>();
                        string folderNames = string.Empty;
                        string empNo = empFolder.Name;

                        foreach (DirectoryInfo direcChild in empFolder.GetDirectories())
                        {
                            foreach (DirectoryInfo direcAss in direcChild.GetDirectories())
                            {
                                string assFolderName = ConfigurationManager.AppSettings["modifyRecordFolderName"].ToString();
                                if (direcAss.Name == assFolderName)
                                {
                                    int formerAssAmount = trBll.GetTaskAmount(empNo, "1");
                                    int currentAssAmount = direcAss.GetFileSystemInfos().Length;
                                    bool delFlag0 = trBll.Delete(empNo, "1");
                                    foreach (FileSystemInfo modifyRecord in direcAss.GetFileSystemInfos())
                                    {
                                        bool isFinished0 = modifyRecord.Name.Contains("完成");
                                        trBll.Add(empNo, direcChild.Name + modifyRecord.Name, "0", DateTime.Now.ToString(), null, isFinished0 ? "1" : "0", "1");
                                    }
                                    if (currentAssAmount > formerAssAmount)
                                    {
                                        if (!trBll.RemindIsExist(empNo, "1"))
                                        {
                                            trBll.AddRemind(empNo, "1");
                                        }
                                        else
                                        {
                                            trBll.UpdateRemind(empNo, "1");
                                        }
                                    }
                                }
                            }
                        }
                        //此前数据库中存储的任务数
                        int formerTaskAmount = trBll.GetTaskAmount(empNo, "0");
                        //该员工目录下有几个文件夹（即任务）
                        int currentTaskAmount = empFolder.GetDirectories().Length;

                        int taskWait = 0;
                        //首先清空原有数据
                        bool delFlag = trBll.Delete(empNo, "0");

                        //遍历单个员工的目录
                        foreach (DirectoryInfo direcChild in empFolder.GetDirectories())
                        {
                            folderNames += string.Format("{0}{1}", folderNames, "\r\n");
                            //取目录名称的前10位构成要求完成的时间
                            //string expireDate = DateTime.ParseExact(direcChild.Name.Substring(0, 10), "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture).ToString();
                            string expireDate = string.Empty;
                            if (ConvertToDateTime(direcChild.Name, out expireDate))
                            {

                                string isFinished = direcChild.Name.Contains("完成") ? "1" : "0";
                                string taskType = "0";

                                #region Remarks
                                //bool haveOverdue = false;
                                //string[] strArr = Convert.ToString(ConfigurationManager.AppSettings["overdueInterval"]).Split(',');
                                //int[] overdueInterval = strArr.Select(i => Convert.ToInt32(i)).ToArray();
                                //Array.Sort(overdueInterval);
                                //for (int index = overdueInterval[0]; index < overdueInterval[overdueInterval.Length - 1]; index++)
                                //{
                                //    if (isFinished == "0" &&
                                //        (DateTime.Now.AddMinutes(index * 60) > Convert.ToDateTime(expireDate) &&
                                //         DateTime.Now.AddMinutes(index * 60).AddSeconds(-Convert.ToInt32(ConfigurationManager.AppSettings["interval"])) < Convert.ToDateTime(expireDate))
                                //        )
                                //    {
                                //        taskType = "3";
                                //        haveOverdue = true;
                                //    }
                                //    trBll.Add(empNo, direcChild.Name, "0", DateTime.Now.ToString(), expireDate, isFinished, "3");
                                //}
                                //if (haveOverdue)
                                //{
                                //    trBll.AddRemind(empNo, "3");
                                //}

                                ////新建的任务
                                //bool isExist = trBll.IsExist(empNo, direcChild.Name);
                                //if (!isExist)
                                //{
                                //    trBll.Add(empNo, direcChild.Name, "0", DateTime.Now.ToString(), expireDate, "1");
                                //}

                                ////已完成的任务，将状态置为完成
                                //if (direcChild.Name.Contains("完成"))
                                //{
                                //    string formerFolderName = direcChild.Name.Substring(0, 22);
                                //    int rows = trBll.SetIsFinished(empNo, formerFolderName);
                                //}
                                #endregion

                                trBll.Add(empNo, direcChild.Name, "0", DateTime.Now.ToString(), expireDate, isFinished, taskType);
                                if (isFinished == "0")
                                {
                                    taskWait++;
                                }
                            }
                        }
                        if (formerTaskAmount < currentTaskAmount)
                        {
                            if (!trBll.RemindIsExist(empNo, "0"))
                            {
                                trBll.AddRemind(empNo, "0");
                            }
                            else
                            {
                                trBll.UpdateRemind(empNo, "0");
                            }
                        }
                    }
                }
                empRootFolder = null;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 转化为时间
        /// </summary>
        /// <param name="inputDate">输入的时间</param>
        /// <param name="returnDate">返回的时间</param>
        /// <returns>是否成功</returns>
        private bool ConvertToDateTime(string inputDate,out string returnDate)
        {
            returnDate = string.Empty;
            try
            {
                returnDate = DateTime.ParseExact(inputDate.Substring(0, 10), "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture).ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnStop()
        {
            LogHelper.WriteLine("服务停止");
        }

        /// <summary>
        /// 设置工作内存占用
        /// </summary>
        /// <param name="maxWorkingSet"></param>
        public static void SetWorkingSet(int maxWorkingSet)
        {
            System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)maxWorkingSet;
        }
    }
}
