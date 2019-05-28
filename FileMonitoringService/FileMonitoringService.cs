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
using System.Collections;
using FileMonitoringService.Model;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using FileMonitoringService.CommonHelper;
using System.Xml;
using FileMonitoringService.Model.WebApi;
using Newtonsoft.Json.Linq;

namespace FileMonitoring
{
    public partial class FileMonitoringService : ServiceBase
    {
        public FileMonitoringService()
        {
            InitializeComponent();
        }

        #region Variable Declaration
        System.Timers.Timer timer;
        System.Timers.Timer timer2;
        System.Timers.Timer timer3;
        TaskRemindingBLL trBll = new TaskRemindingBLL();
        TaskAssignConfigBLL tacBll = new TaskAssignConfigBLL();
        TaskAssignConfigDetailsBLL tacdBll = new TaskAssignConfigDetailsBLL();
        RightDownBLL rdBll = new RightDownBLL();
        TaskAssignFailLog tafl = new TaskAssignFailLog();
        TaskAssignFailLogBLL taflBll = new TaskAssignFailLogBLL();
        #endregion

        #region 服务启动
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
            timer.Elapsed += timerTaskMonitoring_Elapsed;
            timer.Enabled = true;

            timer2 = new System.Timers.Timer();
            int interval2 = Convert.ToInt32(ConfigurationManager.AppSettings["interval2"]) * 1000;
            timer2.Interval = interval2;
            timer2.Elapsed += timerCheckIsCreatedTaskFolder_Elapsed;
            timer2.Enabled = true;

            timer3 = new System.Timers.Timer();
            int interval3 = Convert.ToInt32(ConfigurationManager.AppSettings["interval3"]) * 1000;
            timer3.Interval = interval3;
            timer3.Elapsed += timerTaskAutoAssign_Elapsed;
            timer3.Enabled = Convert.ToString(ConfigurationManager.AppSettings["enableAutoAllotment"]) == "1";//根据配置决定是否启用
        }
        #endregion

        #region 服务停止
        protected override void OnStop()
        {
            LogHelper.WriteLine("服务停止");
        }
        #endregion

        #region 定时器事件
        /// <summary>
        /// 定时器事件，监控任务情况
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerTaskMonitoring_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //WriteFileMonitoring();
            //System.Threading.Thread.Sleep(26000);
            TaskMonitoring();
            SetWorkingSet(750000);
        }

        /// <summary>
        /// 监控任务目录是否创建定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerCheckIsCreatedTaskFolder_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckIsCreatedTaskFolder();
            SetWorkingSet(750000);
        }

        /// <summary>
        /// 自动分配任务定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerTaskAutoAssign_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TaskAutoAssign();
            SetWorkingSet(750000);
        }
        #endregion

        #region 任务监控
        /// <summary>
        /// 任务监控，功能：监测并设置任务完成人，新任务、新售后任务的添加以及任务倒计时3小时提醒
        /// </summary>
        private void TaskMonitoring()
        {
            try
            {
                string empNumberEnable = Convert.ToString(ConfigurationManager.AppSettings["empNumberEnable"]);//状态为启用的员工集合
                //DataTable dtCanAssignEmp = tacdBll.GetCanAssignEmployees(" EMPLOYEENO ").Tables[0];
                //string empNumberEnable = string.Empty;
                //dtCanAssignEmp.AsEnumerable().ToList().ForEach(item => empNumberEnable += item.Field<string>("EMPLOYEENO") + ",");【任务文件夹状态监测】数据库中未创建目录的任务编号: 
                empNumberEnable = empNumberEnable.TrimEnd(',');
                //LogHelper.WriteLine("【任务监控】启用监控的员工:" + empNumberEnable);
                string empFolderPath = Convert.ToString(ConfigurationManager.AppSettings["employeePath"]);  //员工目录
                string modifyRecordFolder = ConfigurationManager.AppSettings["modifyRecordFolderName"].ToString();//存放修改记录的文件夹名
                string questionFolder = ConfigurationManager.AppSettings["questionFolderName"].ToString();//疑问记录
                DirectoryInfo empRootFolder = new DirectoryInfo(empFolderPath);
                ProjectBLL pBll = new ProjectBLL();
                DataTable dtProject = pBll.GetProject(" WHERE DATE_SUB(CURDATE(), INTERVAL 60 DAY) <= date(orderdate)");//筛选出60天内的
                if (dtProject != null && dtProject.Rows.Count > 0)
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
                                    //录入人ID
                                    string enteringPerson = dtProject.Rows[i]["enteringPerson"].ToString();
                                    //截止时间
                                    DateTime dtExpire = Convert.ToDateTime(dtProject.Rows[i]["EXPIREDATE"]);
                                    //完成状态
                                    string isFinished = dtProject.Rows[i]["ISFINISHED"].ToString();
                                    // 订单金额
                                    decimal orderAmount = Convert.ToDecimal(dtProject.Rows[i]["orderAmount"]);
                                    //如果数据库中记录的任务名跟当前目录相符
                                    if (taskFolder.Name.StartsWith(taskNO))
                                    {
                                        //下面首先判断该任务是否已经设置了完成人
                                        #region Old  //discard
                                        //string outUserNo = string.Empty;
                                        //bool isSetFnsPerson = pBll.IsSetFinishedPerson(projectID, out outUserNo);
                                        //if (isSetFnsPerson)
                                        //{
                                        //    //如果设置了完成人，再对两次完成人是否一致进行比较，不一致的情况下才更新
                                        //    if (empNo != outUserNo)
                                        //    {
                                        //        //设置完成人
                                        //        bool updateFlag = pBll.UpdateFinishedPerson(empNo, projectID);
                                        //        if (updateFlag)
                                        //        {
                                        //            LogHelper.WriteLine(string.Format("设置了任务[{0}]的完成人为[{1}]", taskNO, empNo));
                                        //        }
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    //设置完成人
                                        //    bool updateFlag = pBll.UpdateFinishedPerson(empNo, projectID);
                                        //    if (updateFlag)
                                        //    {
                                        //        LogHelper.WriteLine(string.Format("设置了任务[{0}]的完成人为[{1}]", taskNO, empNo));
                                        //    }
                                        //}
                                        #endregion
                                        #region New Method ----------
                                        bool isSetFnsPerson2 = pBll.IsSetFinishedPerson2(projectID, empNo);
                                        if (!isSetFnsPerson2)
                                        {
                                            bool setSuccess = pBll.SetFinishedPerson2(projectID, empNo);
                                            if (setSuccess)
                                            {
                                                LogHelper.WriteLine(string.Format("【任务监控】设置了任务【{0}】完成人为【{1}】", taskNO, empNo));
                                                //2017-05-12 21-07-55，添加提醒
                                                // 发送到的员工类型
                                                string toUserType = "2";
                                                // 如果是 2，则赋值为工程师的员工编号；如果不是，则赋值为录入人ID
                                                string userIDorEmpNo = toUserType == "2" ? empNo : enteringPerson;
                                                if (!trBll.IsExist(enteringPerson, taskNO, null, "0", toUserType))
                                                {
                                                    int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNO, null, "0", DateTime.Now.ToString(), null, "0", "0", toUserType);
                                                    if (addTaskRemindingFlag > 0)
                                                    {
                                                        LogHelper.WriteLine("【任务监控】成功添加任务【" + taskNO + "】分配给完成人【" + empNo + "】提醒");
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        //如果文件夹名称中包含“完成”，则说明任务已完成
                                        if (taskFolder.Name.Contains("完成"))
                                        {
                                            //LogHelper.WriteLine("如果文件夹名称中包含“完成”，则说明任务已完成");
                                            bool flagUptFinish = pBll.UpdateFinishedStatus(projectID);
                                            //如果成功更新了完成状态，就添加一条提醒记录 Add By WangYongkun，2017-05-12 10:06:03
                                            if (flagUptFinish)
                                            {
                                                //LogHelper.WriteLine(empNo + "   " + enteringPerson + "   " + taskNO + "   " + DateTime.Now.ToString() + "   ");
                                                if (!trBll.IsExist(enteringPerson, taskNO, null, "0", "1"))
                                                {
                                                    // 计算员工提成金额并录入账户表（2018-05-19 20-30-31）
                                                    SetTransaction(empNo, taskNO, projectID, orderAmount);

                                                    int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNO, null, "0", DateTime.Now.ToString(), null, "1", "0", "1");
                                                    if (addTaskRemindingFlag > 0)
                                                    {
                                                        LogHelper.WriteLine("【任务监控】成功添加任务【" + taskNO + "】已完成提醒");
                                                    }
                                                }
                                            }
                                        }
                                        //目录名不包含“完成”，并且数据库中也显示是未完成
                                        else if (isFinished != "1")
                                        {
                                            //判断截止时间进行倒计时提醒
                                            TimeSpan ts = dtExpire - DateTime.Now;
                                            //LogHelper.WriteLine("dtExpire:" + dtExpire + "  " + DateTime.Now);
                                            if (ts.TotalHours < 3 && ts.TotalMinutes > 170)
                                            {
                                                //LogHelper.WriteLine($"dtExpire: {dtExpire.ToLongDateString()}, tsTotalHours: {ts.TotalHours}, tsTotalMinutes: {ts.TotalMinutes}");
                                                //LogHelper.WriteLine("执行倒计时1小时体内 empNO  taskNO  " + empNo + "   " + taskNO);
                                                if (!trBll.IsExist(empNo, taskNO, null, "2", "2"))
                                                {
                                                    int addCountdown = trBll.Add(empNo, enteringPerson, taskNO, null, "0", DateTime.Now.ToString(), dtExpire.ToString(), "0", "2", "2");
                                                    if (addCountdown > 0)
                                                    {
                                                        LogHelper.WriteLine("【任务监控】成功添加任务【" + taskNO + "】倒计时3小时提醒");
                                                    }
                                                }
                                            }
                                        }

                                        // 2018-06-24 16:49，如果目录名不包含完成二字，但数据库中状态为已完成，说明是人工设置的完成，需要记录完成人账户
                                        if (!taskFolder.Name.Contains("完成") && isFinished == "1")
                                        {
                                            if (!trBll.IsExist(enteringPerson, taskNO, null, "0", "1"))
                                            {
                                                // 计算员工提成金额并录入账户表（2018-05-19 20-30-31）
                                                SetTransaction(empNo, taskNO, projectID, orderAmount);

                                                int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNO, null, "0", DateTime.Now.ToString(), null, "1", "0", "1");
                                                if (addTaskRemindingFlag > 0)
                                                {
                                                    LogHelper.WriteLine("【任务监控】成功添加任务【" + taskNO + "】已完成提醒");
                                                }
                                            }
                                        }
                                    }
                                }

                                #region 修改和疑问记录文件夹
                                //遍历“修改记录”和“疑问记录”文件夹，这个循环用来检测记录的完成状态
                                foreach (DirectoryInfo taskFolderChild in taskFolder.GetDirectories())
                                {
                                    // 任务类型，0：新任务，1：新的修改，2：修改预警，3：任务待分配提醒。4：新的疑问提醒，5：新的疑问答复。
                                    if (taskFolder.Name.Length >= 22)
                                    {
                                        #region 修改记录
                                        //如果遍历到是修改记录文件夹
                                        //LogHelper.WriteLine("如果遍历到是修改记录文件夹.taskFolderChild.Name:" + taskFolderChild.Name);
                                        if (taskFolderChild.Name.Trim() == modifyRecordFolder)
                                        {
                                            //LogHelper.WriteLine("taskFolderChild.Name.Trim() == modifyRecordFolder");
                                            foreach (FileSystemInfo modifyFileOrFolder in taskFolderChild.GetFileSystemInfos())
                                            {
                                                try
                                                {
                                                    //获取任务目录的原始文件名（因为任务文件夹名称会发生变动，因此取前26个不会变动的字符）
                                                    //LogHelper.WriteLine("taskFolder.Name.Substring(0, 26);");
                                                    string taskNoOriginal = taskFolder.Name.Substring(0, 22);
                                                    //修改记录文件或文件夹（原始名）
                                                    string modfItem = Path.GetFileNameWithoutExtension(modifyFileOrFolder.Name.Trim());
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
                                                    string prjMdfFolder = modfItem;// string.Empty;
                                                    int reviewStatus = 0;
                                                    bool isExistModifyTask = pBll.IsExistModifyTask(taskNoOriginal, modfItem, out projectModifyID, /*out prjMdfFolder,*/ out reviewStatus);
                                                    //LogHelper.WriteLine(string.Format("taskNoOriginal: {0}, projectModifyID: {1}, prjMdfFolder: {2},  reviewStatus:  {3}", taskNoOriginal, projectModifyID,
                                                    //    prjMdfFolder, reviewStatus));
                                                    //LogHelper.WriteLine("isExistModify1:" + isExistModify1);

                                                    //根据任务编号在dtProject中找出对应的记录
                                                    DataRow[] dr = dtProject.Select(string.Format(" taskNO = '{0}'", taskNoOriginal));
                                                    //LogHelper.WriteLine("DataRow[] dr的长度：" + dr.Length);
                                                    string projectID = string.Empty;
                                                    //录入人
                                                    string enteringPerson = string.Empty;
                                                    if (dr.Length > 0)
                                                    {
                                                        projectID = dr[0]["ID"].ToString();
                                                        enteringPerson = dr[0]["ENTERINGPERSON"].ToString();
                                                    }
                                                    //如果projectID为空，就跳过本次循环执行下一次循环
                                                    if (string.IsNullOrEmpty(projectID))
                                                    {
                                                        continue;
                                                    }

                                                    //存在修改任务
                                                    //LogHelper.WriteLine("isExistModify1: " + isExistModify1 + "   taskNoOriginal: " + taskNoOriginal + "         modfItem:" + modfItem + "  projectModifyID: " + projectModifyID);
                                                    if (!isExistModifyTask)
                                                    {
                                                        //不包含“完成”二字，即新的修改任务
                                                        if (!modfItem.Contains("完成"))
                                                        {
                                                            bool isExistNewModifyTask = pBll.IsExistNewModifyScript(projectID, modfItem);
                                                            //LogHelper.WriteLine("projectID: " + projectID + "     modfItem:  " + modfItem);
                                                            if (!isExistNewModifyTask)
                                                            {
                                                                bool addFlag = pBll.AddProjectModify(projectID, modfItem, 0, 1, DateTime.Now);
                                                                //LogHelper.WriteLine("addFlag: " + addFlag + "     projectID: " + projectID + "    modfItem: " + modfItem);
                                                                if (addFlag)
                                                                {
                                                                    LogHelper.WriteLine(string.Format("【任务监控】任务【{0}】的新修改记录【{1}】创建成功", taskNoOriginal, modfItem));
                                                                    if (!trBll.IsExist(enteringPerson, taskNoOriginal, modfItem, "0", "2"))
                                                                    {
                                                                        int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNoOriginal, modfItem, "0", DateTime.Now.ToString(), null, "0", "1", "2");
                                                                        if (addTaskRemindingFlag > 0)
                                                                        {
                                                                            LogHelper.WriteLine(string.Format("【任务监控】新的售后任务【{0}】-【{1}】的提醒添加成功", taskNoOriginal, modfItem));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //否则是修改任务完成了
                                                        else // 20170620 移到这里
                                                        {
                                                            bool isExistFinalModifyScript = pBll.IsExistFinalModifyScript(projectID, modfItem);
                                                            //LogHelper.WriteLine("isExistFinalModifyScript:" + isExistFinalModifyScript +"       ProjectID: "+ projectID+"     modfItem:" + modfItem);
                                                            if (!isExistFinalModifyScript)
                                                            {
                                                                bool addFlag = pBll.AddProjectModify(projectID, modfItem, 1, 1, DateTime.Now);
                                                                if (addFlag)
                                                                {
                                                                    LogHelper.WriteLine(string.Format("【Success】ID【{0}】任务修改记录的完成稿【{1}】创建成功", projectID, modfItem));
                                                                    //LogHelper.WriteLine("判断售后任务之前：enteringPerson:" + enteringPerson);
                                                                    //Below were Added By WangYongkun，2017-05-12 10:06:03
                                                                    if (!trBll.IsExist(enteringPerson, taskNoOriginal, modfItem, "1", "1"))
                                                                    {
                                                                        int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNoOriginal, modfItem, "0", DateTime.Now.ToString(), null, "1", "1", "1");
                                                                        if (addTaskRemindingFlag > 0)
                                                                        {
                                                                            LogHelper.WriteLine(string.Format("【任务监控】售后完成任务【{0}】-【{1}】的提醒添加成功", taskNoOriginal, modifyFileOrFolder.Name));
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    LogHelper.WriteLine(string.Format("【任务监控】ID【{0}】任务修改记录的完成稿【{1}】创建失败", projectID, modifyFileOrFolder.Name));
                                                                }
                                                            }
                                                        }

                                                        // 2018-06-24 20-19-08，不需要审核功能，无所谓了
                                                        ////如果状态是未审核
                                                        //if (reviewStatus == 0)
                                                        //{
                                                        //    bool isSuccessSet = pBll.SetReviewPass(projectModifyID);
                                                        //    if (isSuccessSet)
                                                        //        LogHelper.WriteLine(string.Format("【任务监控】成功设置任务ID为【{0}】的修改任务审核状态", projectModifyID));
                                                        //    else
                                                        //        LogHelper.WriteLine(string.Format("【任务监控】设置任务ID为【{0}】的修改任务审核状态失败", projectModifyID));
                                                        //}
                                                    }


                                                    //LogHelper.WriteLine("开始监测修改截止时间");
                                                    DataSet dsExpireDate = pBll.GetModifyTaskByPrjIdAndTaskName(projectID, modfItem);
                                                    if (dsExpireDate != null && dsExpireDate.Tables.Count > 0 && dsExpireDate.Tables[0].Rows.Count > 0)
                                                    {
                                                        if (!string.IsNullOrEmpty(Convert.ToString(dsExpireDate.Tables[0].Rows[0]["expireDate"])))
                                                        {
                                                            //LogHelper.WriteLine("修改：" + dsExpireDate.Tables[0].Rows[0]["expireDate"]);
                                                            DateTime dtExpireDate = Convert.ToDateTime(dsExpireDate.Tables[0].Rows[0]["expireDate"]);
                                                            //判断截止时间进行倒计时提醒
                                                            TimeSpan ts2 = dtExpireDate - DateTime.Now;
                                                            //LogHelper.WriteLine(ts2.TotalHours.ToString()+"|"+ts2.TotalMinutes.ToString());
                                                            //LogHelper.WriteLine("dtExpire:" + dtExpire + "  " + DateTime.Now);
                                                            if (ts2.TotalHours < 3 && ts2.TotalMinutes > 170)
                                                            {
                                                                //LogHelper.WriteLine($"dtExpire修改的: {dtExpireDate.ToLongDateString()}, tsTotalHours: {ts2.TotalHours}, tsTotalMinutes: {ts2.TotalMinutes}");
                                                                //LogHelper.WriteLine("执行倒计时1小时体内 empNO  taskNO  " + empNo + "   " + taskNO);
                                                                if (!trBll.IsExist(empNo, modfItem, null, "2", "2"))
                                                                {
                                                                    int addCountdown = trBll.Add(empNo, enteringPerson, modfItem, null, "0", DateTime.Now.ToString(), dtExpireDate.ToString(), "0", "2", "2");
                                                                    if (addCountdown > 0)
                                                                    {
                                                                        LogHelper.WriteLine("【任务监控】成功添加任务【" + modfItem + "】倒计时3小时提醒");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    //如果目录名包含“完成”二字并且是以“修改n”打头的，则说明该修改任务已完成，将其录入数据库
                                                    //LogHelper.WriteLine("modifyFolders.Name:" + modifyFolders.Name + "    modfItem:" + modfItem + "   prjMdfFolder:" + prjMdfFolder);
                                                    //if (modfItem.Contains("完成") /*&& modifyFolders.Name   modfItem.StartsWith(prjMdfFolder)*/)
                                                    //{
                                                    //    //LogHelper.WriteLine("modifyFolders.Name.Contains(\"完成\") && modifyFolders.Name == modfItem.Replace(\"完成\", string.Empty)");

                                                    //    //移走 20170620  上面
                                                    //}
                                                }
                                                catch (Exception ex)
                                                {
                                                    LogHelper.WriteLine("【任务监控】错误：" + ex.Message + "\r\n" + ex.StackTrace);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 疑问记录
                                        else if (taskFolderChild.Name.Trim() == questionFolder)
                                        {
                                            //LogHelper.WriteLine("taskFolderChild.Name.Trim() == modifyRecordFolder");
                                            foreach (FileSystemInfo questionFileOrFolder in taskFolderChild.GetFileSystemInfos())
                                            {
                                                try
                                                {
                                                    //LogHelper.WriteLine("taskFolder.Name.Substring(0, 26);");
                                                    string taskNoOriginal = taskFolder.Name.Substring(0, 22);
                                                    //疑问记录文件夹（原始名）
                                                    string qstItem = Path.GetFileNameWithoutExtension(questionFileOrFolder.Name.Trim());
                                                    //LogHelper.WriteLine("modfItem：" + modfItem);
                                                    string projectQuestionID = string.Empty;
                                                    string prjQstFolder = qstItem;// string.Empty;
                                                    int reviewStatus = 0;
                                                    bool isExistModifyTask = pBll.IsExistModifyTask(taskNoOriginal, qstItem, out projectQuestionID, /*out prjMdfFolder,*/ out reviewStatus);
                                                    //LogHelper.WriteLine(string.Format("taskNoOriginal: {0}, projectModifyID: {1}, prjMdfFolder: {2},  reviewStatus:  {3}", taskNoOriginal, projectModifyID,
                                                    //    prjMdfFolder, reviewStatus));
                                                    //LogHelper.WriteLine("isExistModify1:" + isExistModify1);

                                                    //根据任务编号在dtProject中找出对应的记录
                                                    DataRow[] dr = dtProject.Select(string.Format(" taskNO = '{0}'", taskNoOriginal));
                                                    //LogHelper.WriteLine("DataRow[] dr的长度：" + dr.Length);
                                                    string projectID = string.Empty;
                                                    //录入人
                                                    string enteringPerson = string.Empty;
                                                    if (dr.Length > 0)
                                                    {
                                                        projectID = dr[0]["ID"].ToString();
                                                        enteringPerson = dr[0]["ENTERINGPERSON"].ToString();
                                                    }

                                                    //如果projectID为空，就跳过本次循环执行下一次循环
                                                    if (string.IsNullOrEmpty(projectID))
                                                    {
                                                        continue;
                                                    }
                                                    string fileCatWhere = " PROJECTID = '" + projectID + "' AND FOLDERNAME = '" + qstItem + "'";
                                                    FileCategory fileCategory = new FileCategoryBLL().GetModelList(fileCatWhere, string.Empty).FirstOrDefault();
                                                    string createUser = fileCategory.CREATEUSER;
                                                    string taskType = fileCategory.CATEGORY == "5" ? "4" : "5";
                                                    string toUserId = string.Empty;
                                                    string toUserType = string.Empty;
                                                    // 如果创建目录的人跟项目录入人（即客服）是同一人，说明是是客服创建的。因此，该提醒需发送给工程师（即项目完成者）。
                                                    if (enteringPerson == createUser)
                                                    {
                                                        toUserType = "2"; // 发送给工程师
                                                    }
                                                    // 否则的话，说明是工程师的疑问，需发送给客服
                                                    else
                                                    {
                                                        toUserType = "1"; // 发送给客服
                                                    }

                                                    // 数据库中不存在该疑问记录
                                                    //LogHelper.WriteLine("isExistModify1: " + isExistModify1 + "   taskNoOriginal: " + taskNoOriginal + " modfItem:" + modfItem + "  projectModifyID: " + projectModifyID);
                                                    if (!isExistModifyTask)
                                                    {
                                                        //不包含“答复”二字，即新的疑问
                                                        if (!qstItem.Contains("答复"))
                                                        {
                                                            bool isExistNewModifyTask = pBll.IsExistNewModifyScript(projectID, qstItem);
                                                            //LogHelper.WriteLine("projectID: " + projectID + "     modfItem:  " + modfItem);
                                                            if (!isExistNewModifyTask)
                                                            {
                                                                bool addFlag = pBll.AddProjectModify(projectID, qstItem, 0, 1, DateTime.Now);
                                                                //LogHelper.WriteLine("addFlag: " + addFlag + "     projectID: " + projectID + "    modfItem: " + modfItem);
                                                                if (addFlag)
                                                                {
                                                                    LogHelper.WriteLine(string.Format("【任务监控】任务【{0}】的新疑问记录【{1}】创建成功", taskNoOriginal, qstItem));
                                                                    if (!trBll.IsExist(enteringPerson, taskNoOriginal, qstItem, taskType, toUserType))
                                                                    {
                                                                        int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNoOriginal, qstItem, "0", DateTime.Now.ToString(), null, "0", taskType, toUserType);
                                                                        if (addTaskRemindingFlag > 0)
                                                                        {
                                                                            LogHelper.WriteLine(string.Format("【任务监控】新的疑问【{0}】-【{1}】的提醒添加成功", taskNoOriginal, qstItem));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //否则是疑问答复
                                                        else
                                                        {
                                                            bool isExistFinalModifyScript = pBll.IsExistFinalModifyScript(projectID, qstItem);
                                                            //LogHelper.WriteLine("isExistFinalModifyScript:" + isExistFinalModifyScript +"       ProjectID: "+ projectID+"     modfItem:" + modfItem);
                                                            if (!isExistFinalModifyScript)
                                                            {
                                                                bool addFlag = pBll.AddProjectModify(projectID, qstItem, 1, 1, DateTime.Now);
                                                                if (addFlag)
                                                                {
                                                                    LogHelper.WriteLine(string.Format("【Success】ID【{0}】疑问答复【{1}】创建成功", projectID, qstItem));
                                                                    //LogHelper.WriteLine("判断售后任务之前：enteringPerson:" + enteringPerson);
                                                                    //Below were Added By WangYongkun，2017-05-12 10:06:03
                                                                    if (!trBll.IsExist(enteringPerson, taskNoOriginal, qstItem, taskType, toUserType))
                                                                    {
                                                                        int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNoOriginal, qstItem, "0", DateTime.Now.ToString(), null, "1", taskType, toUserType);
                                                                        if (addTaskRemindingFlag > 0)
                                                                        {
                                                                            LogHelper.WriteLine(string.Format("【任务监控】疑问答复【{0}】-【{1}】的提醒添加成功", taskNoOriginal, questionFileOrFolder.Name));
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    LogHelper.WriteLine(string.Format("【任务监控】ID【{0}】任务疑问的答复【{1}】创建失败", projectID, questionFileOrFolder.Name));
                                                                }
                                                            }
                                                        }

                                                        ////如果状态是未审核
                                                        //if (reviewStatus == 0)
                                                        //{
                                                        //    bool isSuccessSet = pBll.SetReviewPass(projectQuestionID);
                                                        //    if (isSuccessSet)
                                                        //        LogHelper.WriteLine(string.Format("【任务监控】成功设置任务ID为【{0}】的疑问审核状态", projectQuestionID));
                                                        //    else
                                                        //        LogHelper.WriteLine(string.Format("【任务监控】设置任务ID为【{0}】的疑问审核状态失败", projectQuestionID));
                                                        //}
                                                    }

                                                    //如果目录名包含“完成”二字并且是以“修改n”打头的，则说明该修改任务已完成，将其录入数据库
                                                    //LogHelper.WriteLine("modifyFolders.Name:" + modifyFolders.Name + "    modfItem:" + modfItem + "   prjMdfFolder:" + prjMdfFolder);
                                                    //if (modfItem.Contains("完成") /*&& modifyFolders.Name   modfItem.StartsWith(prjMdfFolder)*/)
                                                    //{
                                                    //    //LogHelper.WriteLine("modifyFolders.Name.Contains(\"完成\") && modifyFolders.Name == modfItem.Replace(\"完成\", string.Empty)");

                                                    //    //移走 20170620  上面
                                                    //}
                                                }
                                                catch (Exception ex)
                                                {
                                                    LogHelper.WriteLine("【任务监控】错误：" + ex.Message + "\r\n" + ex.StackTrace);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 计入员工交易记录、账户记录
        /// </summary>
        /// <param name="employeeNo"></param>
        /// <param name="taskNo"></param>
        /// <param name="orderAmount"></param>
        private void SetTransaction(string employeeNo, string taskNo, string projectId, decimal orderAmount)
        {
            // 计算员工提成金额并录入账户表
            EmployeeProportionBLL empProporBll = new EmployeeProportionBLL();
            TransactionDetailsBLL tdBll = new TransactionDetailsBLL();
            try
            {
                EmployeeProportion empPrp = empProporBll.GetModelListByEmployeeNo(employeeNo).FirstOrDefault();
                if (empPrp != null)
                {
                    bool updTransacDetails = false;
                    bool isRenewPropor = false;
                    string employeeID = empPrp.EMPLOYEEID;
                    decimal proportion = 0m;
                    ProjectProportion projectProportion = new ProjectProportionBLL().GetModelList(" projectId = '" + projectId + "'").FirstOrDefault();
                    if (projectProportion != null)
                    {
                        proportion = projectProportion.PROPORTION ?? 0m;
                        isRenewPropor = true;
                    }
                    else
                    {
                        proportion = empProporBll.CalculateProportionByEmployeeID(employeeID);
                    }
                    decimal transactionAmount = orderAmount * proportion;
                    string renewMsg = isRenewPropor ? "(改后)" : string.Empty;
                    string description = string.Format("任务{0}{1}", taskNo, renewMsg);
                    // 交易记录
                    DateTime transactionDate = DateTime.Now;
                    DateTime createDate = DateTime.Now;
                    string where = string.Format(" PROJECTID = '" + projectId + "' AND employeeId = '{1}'", taskNo, employeeID);
                    TransactionDetails transac = tdBll.GetModelList(where).FirstOrDefault();
                    // 如果交易记录表中没有满足员工 ID 和项目 ID 的记录，说明是非转移任务
                    if (transac == null || string.IsNullOrEmpty(transac.ID))
                    {
                        transac = new TransactionDetails();
                        transac.ID = Guid.NewGuid().ToString();
                        transac.TRANSACTIONAMOUNT = transactionAmount;
                        transac.TRANSACTIONDESCRIPTION = description;
                        transac.TRANSACTIONPROPORTION = proportion;
                        transac.TRANSACTIONDATE = transactionDate;
                        transac.PLANDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        transac.CREATEDATE = createDate;
                        transac.TRANSACTIONTYPE = 7; // 项目提成
                        transac.EMPLOYEEID = employeeID;
                        transac.PROJECTID = projectId;
                        // 如果交易记录录入成功，继续更新账户金额
                        if (tdBll.Add(transac))
                        {
                            updTransacDetails = true;
                        }
                    }
                    // 经过分部领导转移的任务
                    else
                    {
                        // 更新此前在任务转移时已经录入数据库中的ISDELETED标识暂时置为1的数据
                        decimal proportionCalculated = transac.TRANSACTIONAMOUNT ?? 0 / orderAmount;

                        transac.TRANSACTIONDESCRIPTION = description;
                        transac.TRANSACTIONPROPORTION = proportionCalculated;
                        transac.TRANSACTIONDATE = DateTime.Now;
                        transac.PLANDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        transac.ISDELETED = false;
                        // 更新交易记录成功，则继续更新员工账户
                        if (tdBll.Update(transac))
                        {
                            updTransacDetails = true;
                        }
                    }
                    if (updTransacDetails)
                    {
                        // 员工账户
                        LogHelper.WriteLine(string.Format("任务{0}交易记录录入成功", taskNo));
                        EmployeeAccountBLL empAcctBll = new EmployeeAccountBLL();
                        EmployeeAccount empAcct = empAcctBll.GetModelList(" employeeID = '" + employeeID + "'").FirstOrDefault();
                        empAcct.AMOUNT = empAcct.AMOUNT + transac.TRANSACTIONAMOUNT;
                        empAcct.EMPLOYEEID = employeeID;
                        empAcct.LASTUPDATEDATE = DateTime.Now;
                        if (empAcctBll.Update(empAcct))
                        {
                            LogHelper.WriteLine(string.Format("员工{0}账户更新成功", employeeNo));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine($"账户更新异常：{ex.Message}, {ex.StackTrace}");
            }
        }
        #endregion

        #region 监测任务目录是否忘记创建
        /// <summary>
        /// 监测任务目录是否创建
        /// </summary>
        private void CheckIsCreatedTaskFolder()
        {
            try
            {
                //新任务分配目录
                string taskAllotmentPath = ConfigurationManager.AppSettings["taskAllotmentPath"].ToString();
                //LogHelper.WriteLine("taskAllotPath:   " + taskAllotmentPath);
                ProjectBLL pBll = new ProjectBLL();
                //获取尚未分配的新任务
                DataTable dt = pBll.GetNotCreatedFolderTask(0);
                //存储服务器硬盘中现有的未分配任务目录的集合
                Dictionary<string, string> dicTasks = new Dictionary<string, string>();
                //集合的键索引
                int folderIndex = 0;

                if (Directory.Exists(taskAllotmentPath))
                {
                    //任务分配目录
                    DirectoryInfo dirAllotmentPath = new DirectoryInfo(taskAllotmentPath);
                    //遍历新任务待分配目录
                    foreach (DirectoryInfo allotmentTask in dirAllotmentPath.GetDirectories())
                    {
                        if (!dicTasks.ContainsKey(folderIndex.ToString()))
                        {
                            dicTasks.Add(folderIndex.ToString(), allotmentTask.Name);
                            //LogHelper.WriteLine("【任务文件夹状态监测】待分配任务目录: " + allotmentTask.Name);
                            folderIndex++;
                        }
                    }
                }
                //LogHelper.WriteLine("【任务文件夹状态监测】待分配任务总数: " + dicTasks.Count);
                //新的学生任务待分配目录
                string studentTaskAllotmentPath = ConfigurationManager.AppSettings["studentTaskAllotmentPath"].ToString();//学生任务分配目录
                //判断学生任务待分配目录是否存在（因为这个目录随时可能取消）
                //if (Directory.Exists(studentTaskAllotmentPath))
                //{
                //    DirectoryInfo dirStuAllotmentPath = new DirectoryInfo(studentTaskAllotmentPath);
                //    foreach (DirectoryInfo stuAllotmentTask in dirStuAllotmentPath.GetDirectories())
                //    {
                //        if (!dicTasks.ContainsKey(folderIndex.ToString()))
                //        {
                //            dicTasks.Add(folderIndex.ToString(), stuAllotmentTask.Name);
                //            LogHelper.WriteLine("【任务文件夹状态监测】学生待分配任务目录::    " + stuAllotmentTask.Name);
                //            folderIndex++;
                //        }
                //    }
                //}
                //LogHelper.WriteLine("【任务文件夹状态监测】学生待分配任务总数:   " + dicTasks.Count);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }
                //遍历DataTable，设置相应任务的“是否创建目录”属性
                //LogHelper.WriteLine("【任务文件夹监测】数据库中发现的尚未分配的新任务数量：" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //二层循环，遍历Dictionary
                    foreach (KeyValuePair<string, string> kv in dicTasks)
                    {
                        //如果集合中当前键值对的值号是以DataTable中的任务编打头的
                        string taskNo = dt.Rows[i]["TASKNO"].ToString();
                        //LogHelper.WriteLine("【任务文件夹监测】数据库中存在但未创建目录的任务: " + taskNo);
                        if (kv.Value.StartsWith(taskNo))
                        {
                            string projectID = dt.Rows[i]["ID"].ToString();
                            bool flag = pBll.SetIsCreatedFolder(projectID);
                            if (flag)
                            {
                                // 是否完成设为 0
                                string isFinished = "0";
                                // 任务类型为 3，新任务提醒 的类型
                                string taskType = "3";
                                // 发送给 userType 为 0 ，即管理员
                                string toUserType = "0";
                                if (!trBll.IsExist(string.Empty, taskNo, null, taskType, toUserType))
                                {
                                    int addCountdown = trBll.Add(string.Empty, string.Empty, taskNo, null, "0", DateTime.Now.ToString(), null, isFinished, taskType, toUserType);
                                    if (addCountdown > 0)
                                    {
                                        LogHelper.WriteLine("【任务文件夹监测】成功添加任务【" + taskNo + "】待分配的提醒");
                                    }
                                }
                            }
                            else
                            {
                                LogHelper.WriteLine(string.Format("【任务文件夹监测】设置任务目录是否创建属性失败，任务ID：{0}", projectID));
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine("【任务文件夹监测】" + ex.Message + ex.StackTrace);
            }
        }
        #endregion

        #region 任务分配（2018-03-11）
        /// <summary>
        /// 任务自动分配（2018-03-11）
        /// </summary>
        private void TaskAutoAssign()
        {
            ProjectBLL pBll = new ProjectBLL();
            try
            {
                //总体思路：
                //  先遍历所有未分配的任务；
                //  根据遍历到的任务，对所有员工当前的任务时间情况做判断；
                //  找出时间充足的员工，对这些员工进行综合评分；
                //  将任务分配给得分最高者

                // 获取尚未分配并且资料已上传完成的任务
                DataTable dtPrjNotAllot = pBll.GetNotCreatedFolderTask(1);

                #region 遍历待分配任务列表
                if (dtPrjNotAllot == null || dtPrjNotAllot.Rows.Count == 0)
                {
                    LogHelper.WriteLine("【自动分配】暂无待分配任务");
                    return;
                }

                string access_token = DingTalkHelper.GetAccessToken();
                if (string.IsNullOrEmpty(access_token))
                {
                    LogHelper.WriteLine("【自动分配】钉钉接口调用失败，access_token is null.");
                    return;
                }

                //LogHelper.WriteLine("获取排班信息");
                #region 获取所有员工未来 15 天的排班信息
                List<Schedule> lstSchedule = new List<Schedule>();  //所有员工未来 15 天的排班列表
                for (int attendDay = 0; attendDay <= 15; attendDay++)
                {
                    string attendsResult = AttendsListSchedule(access_token, DateTime.Now.AddDays(attendDay));
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(attendsResult);

                    if (xmldoc == null)
                    {
                        LogHelper.WriteLine("attendsResult is null");
                        return;
                    }
                    if (xmldoc.DocumentElement.Name.Equals("error_response"))
                    {
                        string sub_msg = string.Empty;
                        sub_msg = xmldoc.SelectSingleNode("error_response")["sub_msg"].InnerText;
                        LogHelper.WriteLine(sub_msg);
                    }
                    else if (xmldoc.DocumentElement.Name.Equals("dingtalk_smartwork_attends_listschedule_response"))
                    {
                        XmlNodeList nodeList = xmldoc.SelectNodes("//at_schedule_for_top_vo");
                        for (int schIndex = 0; schIndex < nodeList.Count; schIndex++)
                        {
                            Schedule schedule = new Schedule();
                            schedule.Check_type = nodeList[schIndex]["check_type"].InnerText;
                            schedule.Class_id = nodeList[schIndex]["class_id"].InnerText;
                            schedule.Class_setting_id = nodeList[schIndex]["class_setting_id"].InnerText;
                            schedule.Group_id = nodeList[schIndex]["group_id"].InnerText;
                            schedule.Plan_check_time = Convert.ToDateTime(nodeList[schIndex]["plan_check_time"].InnerText);
                            schedule.Plan_id = nodeList[schIndex]["plan_id"].InnerText;
                            schedule.Userid = nodeList[schIndex]["userid"].InnerText;
                            lstSchedule.Add(schedule);
                        }
                    }
                }
                #endregion
                //LogHelper.WriteLine("排班信息结束");

                //LogHelper.WriteLine("请假信息开始");
                #region 获取所有员工的请假信息
                List<Leave> lstLeaves = new List<Leave>();
                string leaveResult = LeaveProcesses(access_token);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(leaveResult);
                LogHelper.WriteLine("leaveResult: " + leaveResult);
                try
                {
                    if (xmlDoc != null && xmlDoc.DocumentElement.Name.Equals("dingtalk_smartwork_bpms_processinstance_list_response"))
                    {
                        XmlNode xmlNodeOfList = xmlDoc.SelectSingleNode("//list");
                        if (xmlNodeOfList != null)
                        {
                            XmlNodeList nodeList = xmlNodeOfList.ChildNodes;
                            foreach (XmlNode xmlNode in nodeList)
                            {
                                Leave leave = new Leave();
                                if (xmlNode["form_component_values"]["form_component_value_vo"]["value"] != null)
                                {
                                    string values = xmlNode["form_component_values"]["form_component_value_vo"]["value"].InnerText;
                                    string[] array = values.TrimStart('[').TrimEnd(']').Replace("\"", string.Empty).Split(',');
                                    leave.fromDatetime = Convert.ToDateTime(array[0]);
                                    leave.toDatetime = Convert.ToDateTime(array[1]);
                                    leave.totalHours = Convert.ToInt32(array[2]) * 9;//这里需要调整一下，获取员工的每天实际有效工作时间
                                    leave.userId = xmlNode["originator_userid"] != null ? xmlNode["originator_userid"].InnerText : string.Empty;
                                    lstLeaves.Add(leave);
                                }
                            }
                        }
                    }
                }
                catch (Exception exSmartWork)
                {
                    LogHelper.WriteLine("考勤获取错误：" + exSmartWork.Message);
                }
                #endregion
                //LogHelper.WriteLine("请假信息结束");

                // 存储已经分配过的任务ID
                List<string> lstProjectID = new List<string>();
                DataTable dtNotFinishProjects = tacdBll.GetNotFinishProjects().Tables[0];
                // 工时倍数
                DataTable dtEmpTimeMultiple = tacBll.GetList(string.Empty).Tables[0];
                // 所有员工对应的专业列表
                DataTable dtToAllotEmployeesSpecialtiesDetails = tacdBll.GetToAllotEmployeesSpecialties().Tables[0];
                //// 分配失败的任务集合，key 是任务编号，value 是失败原因
                //Dictionary<string, string> dicFailAssign = new Dictionary<string, string>();
                //// 分配失败的原因集合
                //List<AssignFail> lstAssignFail = new List<AssignFail>();
                //设置了自动分配任务的员工
                DataTable dtAutoAssignEmployees = tacdBll.GetCanAssignEmployees(" employeeNo").Tables[0];
                DictionaryIndexer dicIndexerEmpMap = new DictionaryIndexer();
                dtAutoAssignEmployees.AsEnumerable().ToList().ForEach(item => dicIndexerEmpMap[Convert.ToString(item["ID"])] = item["employeeNo"].ToString());
                for (int i = 0; i < dtPrjNotAllot.Rows.Count; i++)
                {
                    string projectID = dtPrjNotAllot.Rows[i]["ID"].ToString();
                    if (lstProjectID.Contains(projectID))
                    {
                        //continue;
                    }
                    string taskNo = dtPrjNotAllot.Rows[i]["TaskNo"].ToString();
                    LogHelper.WriteLine("【自动分配】当前遍历到的任务编号:  " + taskNo);
                    string enteringPerson = dtPrjNotAllot.Rows[i]["enteringPerson"].ToString();
                    int newTaskNeedHours = Convert.ToInt32(dtPrjNotAllot.Rows[i]["timeNeeded"]);            //任务所需的小时数
                    DateTime dtExpire = Convert.ToDateTime(dtPrjNotAllot.Rows[i]["EXPIREDATE"]);     //任务的截止时间（客户要求的交稿时间）
                    string wangwangName = dtPrjNotAllot.Rows[i]["WANGWANGNAME"].ToString();
                    // 等待分配的员工 ID 列表
                    List<string> lstEmployeeIDsForMarking = new List<string>();

                    // 任务所需的专业
                    List<string> lstPrjNeedsSpc = new List<string>();
                    // 员工掌握的专业
                    List<string> lstEmpCanDoSpc = new List<string>();

                    // 权值及计算分数对象
                    Weight weightGlobal = new Weight();
                    weightGlobal.TaskNeedHours = newTaskNeedHours;
                    List<Weight> lstWeightForTimes = new List<Weight>();
                    //LogHelper.WriteLine("自动分配员工数量:    " + dtAutoAssignEmployees.Rows.Count);
                    // 首先遍历所有设置了需要自动分配任务的员工，从空闲时间角度筛选出符合条件的员工，将员工 ID 加入列表中
                    for (int autoAssignEmpIndex = 0, dtAutoAssignEmpsCount = dtAutoAssignEmployees.Rows.Count; autoAssignEmpIndex < dtAutoAssignEmpsCount; autoAssignEmpIndex++)
                    {
                        LogHelper.WriteLine("【自动分配】自动分配员工数：" + dtAutoAssignEmpsCount);
                        string empId = dtAutoAssignEmployees.Rows[autoAssignEmpIndex]["ID"].ToString();
                        string dingTalkId = dtAutoAssignEmployees.Rows[autoAssignEmpIndex]["DINGTALKUSERID"].ToString();
                        //获取该员工手中现有的未完成任务的总的所需时间，这里经过讨论决定忽略该员工已完成的进度，即无论他做了多少，只要是还未完成，都认为是尚未开始
                        //int totalNotFinishNeedHours = Convert.ToInt32(Math.Floor(dtNotFinishProjects.AsEnumerable().Where(
                        //                    row => row.Field<string>("finishedPerson") == empId && !lstProjectID.ToArray().Contains(row.Field<string>("prjId")))
                        //                    .Sum(row => row.Field<decimal>("timeNeeded"))));
                        DataTable dtNotFinishProjects02 = tacdBll.GetNotFinishProjects().Tables[0];
                        int totalNotFinishNeedHours = Convert.ToInt32(Math.Floor(dtNotFinishProjects02.AsEnumerable().Where(
                                            row => row.Field<string>("finishedPerson") == empId //&& !lstProjectID.ToArray().Contains(row.Field<string>("prjId"))
                                            ).Sum(row => row.Field<decimal>("timeNeeded"))));
                        LogHelper.WriteLine("【自动分配】" + dtAutoAssignEmployees.Rows[autoAssignEmpIndex]["employeeNo"].ToString() + "尚有任务未完成小时数：" + totalNotFinishNeedHours + "   ,  钉钉ID：   " + dingTalkId);

                        // 计算这个员工 从当前时间开始，到待分配的这个任务所要求的交稿时间这段时间内 一共有多少可用时间
                        double totalDisposableHours = 0;
                        DateTime dtNow = DateTime.Now;
                        // 判断员工今天的下班时间，如果现在还没有到员工的下班时间，那么他今天还是有机会干活的
                        DateTime dtPlanCheckTime = lstSchedule.Where(sch => sch.Userid == dingTalkId && sch.Check_type == "OffDuty" && sch.Plan_check_time.Date == dtNow.Date).Select(sch => sch.Plan_check_time).FirstOrDefault();
                        if (dtPlanCheckTime > dtNow)
                        {
                            totalDisposableHours += (dtPlanCheckTime - dtNow).TotalHours;
                        }
                        //遍历从第二天开始到截止日期的前一天的排班情况，累加可利用的时间
                        for (int daysAttendsIndex = 1; daysAttendsIndex < Math.Floor((dtExpire - dtNow).TotalDays); daysAttendsIndex++)
                        {
                            DateTime dayWhere = dtNow.AddDays(daysAttendsIndex);
                            // 如果在请假列表中，发现请假的起始时间小于等于当前遍历到的时间并且结束时间大于等于当前遍历到的日期，那么说明员工当天请假，则不计入员工可支配时间中
                            if (lstLeaves.Where(leave => leave.fromDatetime <= dayWhere.Date && leave.toDatetime >= dayWhere.Date).Any())
                            {
                                continue;
                            }//★★
                            IEnumerable<Schedule> iSchedule = lstSchedule.Where(sch => sch.Userid == dingTalkId && sch.Plan_check_time.Date == dayWhere.Date);
                            if (iSchedule.Count() > 0)
                            {
                                Schedule onDutySch = iSchedule.Where(sch => sch.Check_type == "OnDuty").FirstOrDefault();
                                Schedule offDutySch = iSchedule.Where(sch => sch.Check_type == "OffDuty").FirstOrDefault();
                                if (onDutySch != null && offDutySch != null)
                                {
                                    totalDisposableHours += (offDutySch.Plan_check_time - onDutySch.Plan_check_time).TotalHours;
                                }
                            }
                        }
                        Schedule scheduleOnExpireDay = lstSchedule.Where(sch => sch.Userid == dingTalkId && sch.Check_type == "OnDuty" && sch.Plan_check_time.Date == dtExpire.Date).FirstOrDefault();
                        if (scheduleOnExpireDay != null)
                        {
                            // 如果在请假列表中，发现请假的起始时间小于等于任务的截止时间且结束时间大于等于任务的截止时间，那么说明员工当天请假，则不计入员工可支配时间中
                            if (!lstLeaves.Where(leave => leave.fromDatetime <= dtExpire.Date && leave.toDatetime >= dtExpire.Date).Any())
                            {
                                DateTime dtScheduleOnExpireDay = scheduleOnExpireDay.Plan_check_time;
                                if (dtExpire > dtScheduleOnExpireDay)
                                {
                                    totalDisposableHours += (dtExpire - dtScheduleOnExpireDay).TotalHours;
                                }
                            }
                        }

                        // 当前员工的工时倍数
                        int timeMultiples = Convert.ToInt32(dtEmpTimeMultiple.AsEnumerable().Where(item => item.Field<string>("employeeID") == empId).Select(item => item.Field<Int64>("TIMEMULTIPLE")).FirstOrDefault());
                        Weight weightForTime = new Weight();
                        weightForTime.EmployeeID = empId;
                        weightForTime.EmpDisposableHours = totalDisposableHours - totalNotFinishNeedHours;       // 在乘以工时倍数之前给 EmpDisposableHours 赋值
                        LogHelper.WriteLine("【自动分配】" + dicIndexerEmpMap[empId] + "可支配小时数：" + weightForTime.EmpDisposableHours);
                        weightForTime.TimeMultiple = timeMultiples;
                        lstWeightForTimes.Add(weightForTime);
                        //totalDisposableHours = timeMultiples > 0 ? totalDisposableHours * timeMultiples : totalDisposableHours; //防止为 0 时，误将总的可支配时长清零
                        // 如果这个员工的可支配时间（从现在开始到新任务截止时间之前的总的工作时间 减去 当前手中未完成任务所需的时间）大于等于新任务所需的时间，那么就将该员工加入到待进一步评分的列表中
                        LogHelper.WriteLine("(" + totalDisposableHours + "  -   " + totalNotFinishNeedHours + "  )   新任务 " + newTaskNeedHours);
                        if ((totalDisposableHours - totalNotFinishNeedHours) >= newTaskNeedHours)
                        {
                            LogHelper.WriteLine("【自动分配】结果（0）：" + (totalDisposableHours - totalNotFinishNeedHours) + ", " + newTaskNeedHours);
                            LogHelper.WriteLine("【自动分配】结果：" + ((totalDisposableHours - totalNotFinishNeedHours) >= newTaskNeedHours).ToString());
                            if (!lstEmployeeIDsForMarking.Contains(empId))
                            {
                                lstEmployeeIDsForMarking.Add(empId);
                                LogHelper.WriteLine("【自动分配】将员工 " + dicIndexerEmpMap[empId] + " 加入了有时间的集合里");
                            }
                        }
                    }

                    #region 判断时间情况并进行专业的匹配
                    // 如果时间充足的员工人数多于 1 人，那么继续进行专业的匹配
                    string employeeNoToLog = string.Empty;
                    LogHelper.WriteLine("【自动分配】时间集合数量：" + lstEmployeeIDsForMarking.Count());
                    if (lstEmployeeIDsForMarking.Count > 0)
                    {
                        #region 进行专业的匹配
                        ProjectSpecialtyBLL prjSpcBll = new ProjectSpecialtyBLL();
                        // 当前任务需要的专业
                        DataTable dtPrjSpecialties = prjSpcBll.GetSpecialtyInnerJoinProject(projectID, string.Empty).Tables[0];
                        // 当前任务需要的专业数组
                        string[] arrayPrjSpecialties = dtPrjSpecialties.AsEnumerable().Select(spc => spc.Field<string>("specialtyid")).ToArray();
                        string[] arrayPrjSpecialtiesName = dtPrjSpecialties.AsEnumerable().Select(spc => spc.Field<string>("specialtyName")).ToArray();
                        string name01 = string.Empty;
                        arrayPrjSpecialtiesName.AsEnumerable().ToList().ForEach(item => name01 += item + ",");
                        LogHelper.WriteLine("【自动分配】任务所需专业： " + name01);
                        // 将查询出来的专业添加到此前的 list 中
                        lstPrjNeedsSpc.AddRange(arrayPrjSpecialtiesName);

                        // 员工匹配成功的专业列表，key是员工ID，value是该员工匹配成功的专业列表
                        Dictionary<string, List<string>> dicEmpSpcsMatched = new Dictionary<string, List<string>>();
                        IEnumerable<IGrouping<string, DataRow>> empSpecialties = dtToAllotEmployeesSpecialtiesDetails.AsEnumerable().GroupBy(row => row.Field<string>("EMPLOYEEID"));
                        Dictionary<string, int> dicMatches = new Dictionary<string, int>();
                        foreach (IGrouping<string, DataRow> groups in empSpecialties)
                        {
                            int matchesCount = 0;
                            string[] arrMatches = groups.Select(g => g.Field<string>("SPECIALTYCATEGORY")).ToArray();
                            matchesCount = arrMatches.Intersect(arrayPrjSpecialties).Count();
                            dicMatches.Add(groups.Key, matchesCount);       // groups.Key 即 员工ID

                            // 将匹配成功的专业列表加入到员工 ID 及其专业列表的集合中
                            List<string> lstMatchedSpc = arrMatches.Intersect(arrayPrjSpecialties).ToList();
                            dicEmpSpcsMatched.Add(groups.Key, lstMatchedSpc);
                            string spcs01 = string.Empty;
                            lstMatchedSpc.ForEach(item => spcs01 += item + ",");
                            LogHelper.WriteLine("【自动分配】匹配成功的专业：" + spcs01);
                            //foreach (dynamic specs in groups)
                            //{
                            //    string ddd = Convert.ToString(specs["SPECIALTYCATEGORY"]);
                            //    //if(groups.Where(row=>row.SPECIALTYCATEGORY).Any
                            //}
                        }
                        // 如果有不止一个最大值，说明有多人具备相同的专业匹配数，那么需要将这几个员工都添加到列表中
                        int maxValueInDicMatches = dicMatches.Max(item => item.Value);
                        if (maxValueInDicMatches == 0)
                        {
                            // 写入分配失败的日志并记录次数=========================================================================================
                            LogHelper.WriteLine("【自动分配】专业最大值没有！！");
                            continue;
                        }
                        // 获得专业匹配数量最大的员工（会有一个或多个）
                        IEnumerable<KeyValuePair<string, int>> kvpMatchesMaxes = dicMatches.Where(item => item.Value == maxValueInDicMatches);
                        // 如果有多个员工有相同的专业匹配数量
                        if (kvpMatchesMaxes.Count() > 1)
                        {
                            LogHelper.WriteLine("【自动分配】多人匹配专业成功：" + kvpMatchesMaxes.Count());
                            List<string> lstIntersectTimeAndSpcMatched = new List<string>();
                            // 找出专业匹配成功数量最多的几位员工
                            var lstMatches = (from mtc in kvpMatchesMaxes
                                              where mtc.Value == (kvpMatchesMaxes.Max(item => item.Value))
                                              select mtc.Key).ToList();
                            //如果有多人具备同样的专业匹配数
                            if (lstMatches.Count() > 1)
                            {
                                lstIntersectTimeAndSpcMatched = lstEmployeeIDsForMarking.Intersect(lstMatches).ToList();
                                lstEmployeeIDsForMarking.Clear();
                                foreach (var matchedEmpID in lstIntersectTimeAndSpcMatched)
                                {
                                    lstEmployeeIDsForMarking.Add(matchedEmpID);
                                }
                            }
                            else if (lstMatches.Count() == 1)
                            {
                                lstEmployeeIDsForMarking.Clear();
                                lstEmployeeIDsForMarking.Add(lstMatches.FirstOrDefault());
                            }
                        }
                        else if (kvpMatchesMaxes.Count() == 1)
                        {
                            lstEmployeeIDsForMarking.Clear();
                            lstEmployeeIDsForMarking.Add(kvpMatchesMaxes.FirstOrDefault().Key);
                        }
                        #endregion

                        #region 判断任务专业及员工掌握的专业
                        if (lstEmployeeIDsForMarking.Count > 0)
                        {
                            // 进入到这里，便开始对一系列的参数进行计算
                            // 0. 是不是忘记专业了？？？？？==========================  已经加上了！！！！！！！！！！
                            // 1. 将回头客、时间空闲、质量分以及预期工资是否达到（包括超出比例）全部拉取出来
                            // 2. 对拉取出来的数据，按照员工ID进行遍历，并结合相应系数，生成员工分数表
                            // 3. 取分数表中分值最高的员工，将任务分配给此人
                            // 公式：时间空闲度 × 时间空闲系数 + 是否回头客 × 回头客系数 + 预期目标（指工资）是否达到 × 预期目标完成系数 × 降权比例（这个比例是动态的，根据现在完成的工资情况） + 质量分
                            string empNos02 = "";
                            lstEmployeeIDsForMarking.ForEach(item => empNos02 += dicIndexerEmpMap[item] + ",");

                            LogHelper.WriteLine("【自动分配】经过时间和专业筛选，共有 " + lstEmployeeIDsForMarking.Count() + " 人可分配，有 " + empNos02);
                            Dictionary<string, decimal> dicEmpScores = new Dictionary<string, decimal>();
                            WeightsConfig weightConfig = new WeightsConfig();
                            WeightsConfigBLL wcBll = new WeightsConfigBLL();
                            List<WeightsConfig> lstWeightConfig = wcBll.GetModelList(string.Empty);
                            decimal? weightQualityProportion = lstWeightConfig.Where(item => item.ITEMNAME.Equals("质量分")).Select(item => item.ITEMVALUE).FirstOrDefault();
                            decimal? weightFreeProportion = lstWeightConfig.Where(item => item.ITEMNAME.Equals("时间空闲")).Select(item => item.ITEMVALUE).FirstOrDefault();
                            decimal? weightObjectiveProportion = lstWeightConfig.Where(item => item.ITEMNAME.Equals("预期目标完成")).Select(item => item.ITEMVALUE).FirstOrDefault();
                            decimal? weightRepeatCustomerProportion = lstWeightConfig.Where(item => item.ITEMNAME.Equals("回头客")).Select(item => item.ITEMVALUE).FirstOrDefault();
                            weightGlobal.QualityScoreRight = weightQualityProportion ?? 1;
                            weightGlobal.FreeTimeRight = weightFreeProportion ?? 1;
                            weightGlobal.ObjectiveRightdown = weightObjectiveProportion ?? 1;
                            //RepeatCustomer repeatCstmGlobal = new RepeatCustomer();
                            //repeatCstmGlobal.Proportion = weightRepeatCustomerProportion ?? 0;

                            #region ==== 回头客 ====
                            string emps = string.Empty;
                            lstEmployeeIDsForMarking.ToList().ForEach(item => emps += "'" + item + "',");
                            emps = emps.TrimEnd(',');
                            // 获取时间充足的员工此前做过的任务，判断是否是老客户
                            DataTable dtPreviousTasks = tacdBll.GetPreviousTask(emps).Tables[0];
                            #endregion

                            #region ==== 质量分（及工时倍数？） ====

                            #endregion

                            #region ==== 员工当月已经达到的工资 ====
                            // 预期工资达标降权配置表
                            List<RightDown> lstRightDown = rdBll.GetModelList(string.Empty);
                            // 员工目标金额表
                            DataTable dtEmpTargetAmount = tacBll.GetList(string.Empty).Tables[0];
                            // 当前符合评分标准的所有员工的当月累计订单金额
                            DataTable dtAllCurrentAmount = tacdBll.GetAllCurrentAmount(emps).Tables[0];
                            // 员工专业、质量分表
                            DataTable dtAllEmpSpecialtiesQualities = tacdBll.GetAllTaskAssignDetails().Tables[0];

                            #endregion

                            LogHelper.WriteLine("【自动分配】员工权值集合开始");
                            // 员工权值集合
                            List<Weight> lstEmpWeight = new List<Weight>();
                            foreach (string itemEmpID in lstEmployeeIDsForMarking)
                            {
                                Weight weight = new Weight();
                                weight.TaskNeedHours = weightGlobal.TaskNeedHours;
                                weight.EmpDisposableHours = lstWeightForTimes.Where(item => item.EmployeeID == itemEmpID).Select(item => item.EmpDisposableHours).FirstOrDefault();// weightGlobal.EmpDisposableHours;
                                weight.TimeMultiple = lstWeightForTimes.Where(item => item.EmployeeID == itemEmpID).Select(item => item.TimeMultiple).FirstOrDefault(); // weightGlobal.TimeMultiple;
                                weight.FreeTimeRight = weightGlobal.FreeTimeRight;
                                weight.QualityScoreRight = weightGlobal.QualityScoreRight;
                                //weight.RepeatCustomer = repeatCstmGlobal;
                                //decimal freeProportion = 0;
                                //decimal objectiveProportion = 0;
                                //LogHelper.WriteLine("emps:   " + emps);
                                //LogHelper.WriteLine("EMPid:   " + itemEmpID);
                                decimal currentAmount = dtAllCurrentAmount.Rows.Count > 0 ?
                                                        (from item in dtAllCurrentAmount.AsEnumerable()
                                                         let afterCalculatedAmount = item.Field<double>("orderAmount") * item.Field<double>("proportion")
                                                         where item.Field<string>("finishedPerson") == itemEmpID
                                                         select Convert.ToDecimal(afterCalculatedAmount)).Sum() : 0M;
                                // 员工的目标金额
                                decimal empTargetAmount = dtEmpTargetAmount.AsEnumerable().Where(item => item.Field<string>("employeeID").Equals(itemEmpID)).Select(item => item.Field<decimal>("targetAmount")).FirstOrDefault();
                                // 员工的超出金额（当前已经达到的订单额 减去 目标金额）
                                decimal exceedAmount = currentAmount - empTargetAmount;
                                decimal exceedProportion = 0;
                                decimal rightDownProportion = 0;
                                // 如果超出了目标金额
                                if (exceedAmount > 0)
                                {
                                    exceedProportion = exceedProportion / empTargetAmount;
                                    rightDownProportion = lstRightDown.Where(item => exceedProportion >= item.FROMVALUE && exceedProportion <= item.TOVALUE).Select(item => item.RIGHTDOWNPERCENT).FirstOrDefault() ?? 0;
                                    weight.ObjectiveIsFinished = true;
                                    weight.ObjectiveFinishedProportion = exceedProportion;
                                    weight.ObjectiveRightdown = rightDownProportion;
                                }

                                weight.EmployeeID = itemEmpID;

                                #region 回头客权值
                                // 回头客权值
                                RepeatCustomer repeatCustomer = new RepeatCustomer();
                                bool isRepeatCustomer = false;
                                isRepeatCustomer = dtPreviousTasks.AsEnumerable().Any(item => item.Field<string>("FINISHEDPERSON") == itemEmpID && item.Field<string>("WANGWANGNAME") == wangwangName);
                                repeatCustomer.IsRepeat = isRepeatCustomer;
                                if (isRepeatCustomer)
                                {
                                    repeatCustomer.Proportion = weightRepeatCustomerProportion ?? 0;
                                }
                                else
                                {
                                    repeatCustomer.Proportion = 0;
                                }
                                weight.RepeatCustomer = repeatCustomer;
                                #endregion

                                // 预期目标完成权值
                                weight.ObjectiveFinishedProportion = weightObjectiveProportion * rightDownProportion ?? 0;

                                //公式：时间空闲度 × 时间空闲系数 + 是否回头客 × 回头客系数 + 
                                // 预期目标（指工资）是否达到 × 预期目标完成系数 × 降权比例（这个比例是动态的，根据现在完成的工资情况） + 质量分
                                decimal qualityScore = 0;
                                var arrEmpSpcsMatched = dicEmpSpcsMatched.Where(item => item.Key.Equals(itemEmpID)).Select(item => item.Value).FirstOrDefault().ToArray();
                                qualityScore = (from empQualityScore in dtAllEmpSpecialtiesQualities.AsEnumerable()
                                                where empQualityScore.Field<string>("employeeID").Equals(itemEmpID) && empQualityScore.Field<Int64>("available").Equals(1)
                                                && arrEmpSpcsMatched.Contains(empQualityScore.Field<string>("SPECIALTYCATEGORY"))
                                                select empQualityScore.Field<decimal>("qualityScore")).Average();
                                // 注：质量分目前没有设置系数

                                weight.QualityScore = qualityScore;
                                //weight.FinalScore = weight.RepeatCustomer.Proportion
                                //                    + weight.ObjectiveFinishedProportion
                                //                    + qualityScore;

                                decimal finalScore = (weight.QualityScore * weight.QualityScoreRight
                                    + Convert.ToDecimal(weight.EmpDisposableHours * weight.TimeMultiple * Convert.ToDouble(weight.FreeTimeRight))
                                    + repeatCustomer.Proportion)
                                    * (1 - weight.ObjectiveRightdown);

                                weight.FinalScore = finalScore;

                                weight.ProjectNeedSpc = lstPrjNeedsSpc;
                                weight.EmpCanDoSpc = dtToAllotEmployeesSpecialtiesDetails.AsEnumerable().Where(item => item.Field<string>("employeeID") == itemEmpID).Select(item => item.Field<string>("SPECIALTYNAME")).ToList();

                                lstEmpWeight.Add(weight);
                                LogHelper.WriteLine("【自动分配】将 " + dicIndexerEmpMap[weight.EmployeeID] + " 加入了最终的分配中");
                                weight = null;
                            }
                            Weight weightFinal = lstEmpWeight.Where(item => item.FinalScore == lstEmpWeight.Max(itemChild => itemChild.FinalScore)).FirstOrDefault();
                            MoveTaskFolder(projectID, taskNo, weightFinal.EmployeeID, enteringPerson, weightFinal);
                            // 将ProjectID加入到list中
                            if (!lstProjectID.Contains(projectID))
                            {
                                lstProjectID.Add(projectID);
                            }

                            // 释放资源
                            dtPreviousTasks = null;
                            dtAllEmpSpecialtiesQualities = null;
                            dtAllCurrentAmount = null;
                            dtEmpTargetAmount = null;
                        }
                        // 没有匹配到掌握任务需要的专业的员工
                        else
                        {
                            // 写入日志，记录没有专业符合的员工（应该不会出现的吧？）
                            string descriptionMessage = "【自动分配】" + taskNo + "没有专业符合的员工";
                            LogHelper.WriteLine(descriptionMessage);
                            RecordFailAssign(projectID, taskNo, descriptionMessage);
                        }
                        #endregion
                    }
                    else
                    {
                        // 写入日志，记录没有时间充足的员工（应该不会出现的吧？）
                        string descriptionMessage = "【自动分配】" + taskNo + "没有时间充足的员工";
                        LogHelper.WriteLine(descriptionMessage);
                        RecordFailAssign(projectID, taskNo, descriptionMessage);
                    }
                    #endregion

                    #region
                    //Dictionary<string, int> dicMatchesOrdered = dicMatches.OrderBy(item => item.Value).ToDictionary(item => item.Key, item => item.Value);
                    ////符合任务专业要求的员工如果有多人（1人以上），则对这些员工进行评分计算。解释：如果一个任务需要土建、安装和装修 3 个专业，下面语句表示同时掌握这 3 门技术的员工不止一人，那么需要取其中分值最高者。
                    //if (dicMatchesOrdered.Select(item => item.Value == arrayPrjSpecialties.Length).Count() > 1)
                    //{

                    //}
                    ////符合任务专业要求的员工只有1人，那么就分配给该员工。解释：同时掌握土建、安装和装修的只有一位员工，那么就直接将任务分配给该员工。
                    //else if (dicMatchesOrdered.Select(item => item.Value == arrayPrjSpecialties.Length).Count() > 0)
                    //{

                    //}
                    ////如果没有符合当前任务的多专业要求的（没有同时掌握土建、安装和装修技能的员工），那么降低要求，找出掌握 2 门（即 3 - 1）技术的员工
                    //else if (dicMatchesOrdered.Select(item => item.Value > 0).Count() > 0)
                    //{

                    //}
                    #endregion
                }
                #endregion
                // ================遍历任务结束===============
                // 需要一个集合记录所有未分配成功的任务并集中提醒
                LogHelper.WriteLine("【自动分配】开始记录分配失败的日志");
                List<TaskAssignFailLog> lstTaskAssignFail = taflBll.GetAssignFailAndCountCanDivByThree();
                LogHelper.WriteLine("【自动分配】分配失败数量：" + lstTaskAssignFail.Count());
                if (lstTaskAssignFail.Count > 0)
                {
                    StringBuilder sbFailItems = new StringBuilder();
                    sbFailItems.Append("以下任务多次分配失败：\r\n");
                    int taflForCount = lstTaskAssignFail.Count;
                    LogHelper.WriteLine("失败次数是 3 的倍数的任务共有 " + lstTaskAssignFail.Count + " 条");
                    for (int taflIndex = 0; taflIndex > taflForCount; taflIndex++)
                    {
                        string taskNoTemp = dtPrjNotAllot.AsEnumerable().Where(item => item.Field<string>("ID") == lstTaskAssignFail[taflIndex].PROJECTID).Select(item => item.Field<string>("taskNo")).FirstOrDefault();
                        LogHelper.WriteLine("分配失败提醒 taskNoTemp：" + taskNoTemp);
                        string failCount = Convert.ToString(lstTaskAssignFail[taflIndex].FAILCOUNT);
                        sbFailItems.Append(taskNoTemp + "：" + failCount + "；\r\n");
                    }
                    LogHelper.WriteLine("测试环境：任务分配失败信息：" + sbFailItems);
                    string adminDingdingID = ConfigurationManager.AppSettings["adminDingdingID"].ToString();
                    //SendDingtalkMessage(access_token, "083837256338737962", sbFailItems.ToString());
                }

                dtPrjNotAllot = null;
                dtNotFinishProjects = null;
                LogHelper.WriteLine("【自动分配】此轮任务分配执行结束");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine("【自动分配】分配执行失败，原因:" + ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 移动任务目录
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="taskNo">任务编号</param>
        /// <param name="employeeID">员工ID</param>
        /// <param name="enteringPerson">录入人ID</param>
        /// <param name="weight">权重对象</param>
        private void MoveTaskFolder(string projectID, string taskNo, string employeeID, string enteringPerson, Weight weight)
        {
            Employee employee = new Employee();
            EmployeeBLL empBll = new EmployeeBLL();
            employee = empBll.GetModel(employeeID);
            ProjectBLL pBll = new ProjectBLL();
            string employeeNo = employee.EMPLOYEENO;
            //新任务分配目录
            string taskAllotmentPath = Convert.ToString(ConfigurationManager.AppSettings["taskAllotmentPath"]);
            //所有员工的父级目录
            string employeePath = Convert.ToString(ConfigurationManager.AppSettings["employeePath"]);
            string fullTaskFolderName = string.Empty;
            //如果当前任务是新任务待分配下的任务，则前面的路径是新任务待分配的路径：taskAllotmentPath
            //if (Directory.Exists(taskAllotmentPath + taskNo))
            //{
            fullTaskFolderName = taskAllotmentPath + taskNo;
            //}
            string fullEmpTaskName = employeePath + "\\" + employee.EMPLOYEENO + "\\" + taskNo;
            //LogHelper.WriteLine("fullEmpTaskName:" + fullEmpTaskName);
            try
            {
                //移动目录
                //MoveFolder(fullTaskFolderName, fullEmpTaskName);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(string.Format("{0}：{1}", "目录移动失败", ex.Message));
                //添加日志
                SystemLog log = new SystemLog();
                log.ID = Guid.NewGuid().ToString("N");
                log.EMPLOYEEID = "66e15576b4a141f8903655256ab6a254";
                log.IPADDRESS = "127.0.0.1";
                log.OPERATETYPE = "1";
                string content = string.Format("目录移动失败：{0}", ex.Message);
                log.OPERATECONTENT = content;
                log.CREATEDATE = DateTime.Now;
                SystemLogBLL slBll = new SystemLogBLL();
                bool flag = slBll.Add(log);
                if (!flag)
                {
                    LogHelper.WriteLine("系统自动分配日志添加失败");
                }
                return;
            }
            //设置任务的完成人
            bool isSetFnsPerson = false;// pBll.IsSetFinishedPerson2(projectID, employeeNo);
            if (!isSetFnsPerson)
            {
                bool setSuccess = true;// pBll.SetFinishedPerson2(projectID, employeeNo);
                if (setSuccess)
                {
                    LogHelper.WriteLine(string.Format("【自动分配】设置了任务[{0}]的完成人为[{1}]", taskNo, employeeNo));
                    //if (!trBll.IsExist(enteringPerson, taskNo, null, "0", "2"))
                    //{
                    //    int addTaskRemindingFlag = trBll.Add(employeeNo, enteringPerson, taskNo, null, "0", DateTime.Now.ToString(), null, "0", "0", "2");
                    //    if (addTaskRemindingFlag > 0)
                    //    {
                    //        LogHelper.WriteLine("成功添加任务[" + taskNo + "]分配给完成人[" + employeeNo + "]的提醒");
                    //    }
                    //}

                    //添加日志
                    SystemLogBLL slBll = new SystemLogBLL();


                    SystemLog log = new SystemLog();
                    log.ID = Guid.NewGuid().ToString("N");
                    log.EMPLOYEEID = "66e15576b4a141f8903655256ab6a254";
                    log.IPADDRESS = "127.0.0.1";
                    log.OPERATETYPE = "1";
                    StringBuilder sbContent = new StringBuilder();
                    sbContent.AppendFormat("系统分配 {0} 给 {1}\r\n", taskNo, employeeNo);
                    //content += string.Format("回头客：{0}\n预期达成：{1}\n质量分：{2}\n最终分：{3}", weight.RepeatCustomer.IsRepeat ? "是" : "否", weight.ObjectiveFinishedProportion, weight.QualityScore, weight.FinalScore);

                    SystemLog log01 = new SystemLog();
                    //LogHelper.WriteLine(" OPERATECONTENT like '" + sbContent.ToString().Replace("\r\n", "") + "%' ");
                    List<SystemLog> lstLog = slBll.GetModelList(" OPERATECONTENT like '" + sbContent.ToString().Replace("\r\n", "") + "%' ");
                    if (lstLog.Count > 0)
                    {
                        return;
                    }

                    string prjNeedSpc = string.Empty;
                    weight.ProjectNeedSpc.ForEach(item => prjNeedSpc += item + "、");
                    prjNeedSpc = prjNeedSpc.TrimEnd('、');
                    string empCanDoSpc = string.Empty;
                    weight.EmpCanDoSpc.ForEach(item => empCanDoSpc += item + "、");
                    empCanDoSpc = empCanDoSpc.TrimEnd('、');
                    sbContent.AppendFormat("任务所需专业：{0}，员工掌握专业：{1}\r\n", prjNeedSpc, empCanDoSpc);
                    sbContent.AppendFormat("专业质量分：{0}\r\n", weight.QualityScore);
                    sbContent.AppendFormat("任务所需时长：{0}，可支配时长：{1}，工时倍数：{2}\r\n", weight.TaskNeedHours, weight.EmpDisposableHours, weight.TimeMultiple);
                    if (weight.ObjectiveIsFinished)
                    {
                        sbContent.AppendFormat("预期目标达成：是，超出比例：{0}，降权比例：{1}\r\n", weight.ObjectiveFinishedProportion, weight.ObjectiveRightdown);
                    }
                    else
                    {
                        sbContent.Append("预期目标完成：否\r\n");
                    }
                    sbContent.AppendFormat("最终分：{0}\r\n", weight.FinalScore);
                    sbContent.Append("公式：（专业质量分×质量分权重＋（可支配时长×空余时长权重×工时倍数）＋回头客权重）×降权比例\r\n");
                    if (weight.ObjectiveIsFinished)
                    {
                        sbContent.Append("（");
                    }
                    sbContent.AppendFormat("{0}×{1}＋（{2}×{3}×{4}）", weight.QualityScore, weight.QualityScoreRight, weight.EmpDisposableHours, weight.FreeTimeRight, weight.TimeMultiple);
                    if (weight.RepeatCustomer.IsRepeat)
                    {
                        sbContent.AppendFormat("＋{0}", weight.RepeatCustomer.Proportion);
                    }
                    if (weight.ObjectiveIsFinished)
                    {
                        sbContent.AppendFormat("）×{0}", 1 - weight.ObjectiveRightdown);
                    }

                    log.OPERATECONTENT = sbContent.ToString();
                    log.CREATEDATE = DateTime.Now;
                    bool flag = slBll.Add(log);
                    if (!flag)
                    {
                        LogHelper.WriteLine("系统自动分配日志添加失败");
                    }
                }
            }
        }

        /// <summary>
        /// 记录分配失败的情况
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="taskNo"></param>
        /// <param name="descriptMessage"></param>
        private void RecordFailAssign(string projectID, string taskNo, string descriptionMessage)
        {
            // 写入日志，汇报所有人都没有时间并记录此轮次数（下次会再加 1 次，至 3 次时，发送钉钉消息给管理员）
            TaskAssignFailLog taskAssignFail = new TaskAssignFailLog();
            List<TaskAssignFailLog> lstTaskAssignFail = new List<TaskAssignFailLog>();
            LogHelper.WriteLine("【任务分配】记录分配失败日志开始");
            lstTaskAssignFail = taflBll.GetModelList(" projectID = '" + projectID + "'");
            if (lstTaskAssignFail.Count() > 0)
            {
                LogHelper.WriteLine("【任务分配】分配失败数量大于0，为" + lstTaskAssignFail.Count());
                taskAssignFail = lstTaskAssignFail.FirstOrDefault();
                taskAssignFail.FAILCOUNT = taskAssignFail.FAILCOUNT + 1;
                taskAssignFail.ISREMIND = 1;
                taflBll.Update(taskAssignFail);
                //if (taskAssignFail.FAILCOUNT % 3 == 0)
                //{
                //string message = string.Format("[{0}] 第 {1} 次分配未果，请注意！", taskNo, taskAssignFail.FAILCOUNT);
                //SendDingtalkMessage(access_token, "083837256338737962", message + " | 原因：" + descriptionMessage);
                //}
            }
            else
            {
                taskAssignFail.ID = Guid.NewGuid().ToString();
                taskAssignFail.PROJECTID = projectID;
                taskAssignFail.FAILCOUNT = 1;
                taskAssignFail.ISREMIND = 0;
                taflBll.Add(taskAssignFail);
            }
            LogHelper.WriteLine("【任务分配】记录分配失败日志结束");
        }

        #region 获取排班信息
        /// <summary>
        /// 根据日期获取排班信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="dtAttendDate"></param>
        /// <returns></returns>
        private string AttendsListSchedule(string access_token, DateTime dtAttendDate)
        {
            string parameters = string.Format("{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}",
                DingTalkHelper.UrlEncodeNew("method"), DingTalkHelper.UrlEncodeNew("dingtalk.smartwork.attends.listschedule"),
                   DingTalkHelper.UrlEncodeNew("session"), DingTalkHelper.UrlEncodeNew(access_token),
                   DingTalkHelper.UrlEncodeNew("v"), DingTalkHelper.UrlEncodeNew("2.0"),
                   DingTalkHelper.UrlEncodeNew("format"), DingTalkHelper.UrlEncodeNew("xml"),
                   DingTalkHelper.UrlEncodeNew("timestamp"), DingTalkHelper.UrlEncodeNew(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            IDingTalkClient client = new DefaultDingTalkClient("https://eco.taobao.com/router/rest?" + parameters);
            SmartworkAttendsListscheduleRequest req = new SmartworkAttendsListscheduleRequest();
            req.WorkDate = dtAttendDate;// DateTime.Now.AddDays(15); // DateTime.Parse("2018-04-10 11:11:11");
            req.Offset = 0L;
            req.Size = 200L;
            SmartworkAttendsListscheduleResponse rsp = client.Execute(req, access_token);
            return rsp != null ? rsp.Body : string.Empty;
        }
        #endregion

        #region 获取请假信息
        /// <summary>
        /// 获取请假信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        private string LeaveProcesses(string access_token)
        {
            string processCode = ConfigurationManager.AppSettings["SmartWorkProcessCode"].ToString();
            string parameters = string.Format("{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}",
                DingTalkHelper.UrlEncodeNew("method"), DingTalkHelper.UrlEncodeNew("dingtalk.smartwork.bpms.processinstance.list"),
                   DingTalkHelper.UrlEncodeNew("session"), DingTalkHelper.UrlEncodeNew(access_token),
                   DingTalkHelper.UrlEncodeNew("v"), DingTalkHelper.UrlEncodeNew("2.0"),
                   DingTalkHelper.UrlEncodeNew("format"), DingTalkHelper.UrlEncodeNew("xml"),
                   DingTalkHelper.UrlEncodeNew("timestamp"), DingTalkHelper.UrlEncodeNew(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            IDingTalkClient client = new DefaultDingTalkClient("https://eco.taobao.com/router/rest?" + parameters);
            SmartworkBpmsProcessinstanceListRequest req = new SmartworkBpmsProcessinstanceListRequest();
            req.ProcessCode = processCode;
            req.StartTime = DateTimeUtility.CSharpTimeToJavaScriptTime(DateTime.Now.AddDays(-1));     //  1514822400000L
            req.Size = 10L;
            req.Cursor = 0L;
            //req.UseridList = "[\"086841065229224690\",\"083837256338737962\"]";
            SmartworkBpmsProcessinstanceListResponse rsp = client.Execute(req, access_token);
            return rsp != null ? rsp.Body : string.Empty;
        }
        #endregion

        #region 发送钉钉消息
        /// <summary>
        /// 发送钉钉消息
        /// </summary>
        /// <param name="access_token">access_token</param>
        /// <param name="dingTalkUserID">接收者钉钉UserId</param>
        /// <param name="messageContent">消息内容</param>
        private void SendDingtalkMessage(string access_token, string dingTalkUserID, string messageContent)
        {
            string externalDingdingUserId = string.Empty;
            string errcode = string.Empty;
            //accessToken = jObj["access_token"].ToString();
            //发送消息接口的请求地址
            string postUrl = string.Format("https://oapi.dingtalk.com/message/send?access_token={0}", access_token);
            //钉钉企业应用id，这个值代表以哪个应用的名义发送消息
            string agentid = ConfigurationManager.AppSettings["agentid"];

            //定义接收者
            StringBuilder toUser = new StringBuilder(dingTalkUserID);
            //如果额外的接收者不为空
            if (!string.IsNullOrEmpty(externalDingdingUserId))
            {
                toUser.Append(externalDingdingUserId);
            }

            string param = "{\"touser\": \"" + toUser.ToString() + "\", \"msgtype\": \"text\", \"agentid\": \"" + agentid + "\",\"text\":{" +
                           "\"content\": \"" + messageContent + "\"}}";
            //接口返回结果
            object msgSendResult = WebServiceHelper.Post(postUrl, param);
            //将Object类型结果转换为JObject
            JObject jsonSendResult = JObject.Parse(msgSendResult.ToString());
            //返回码
            errcode = jsonSendResult["errcode"].ToString();
            //返回码为0，发送成功
            if (errcode == "0")
            {
                LogHelper.WriteLine("发送至高成功");
            }
            else
            {
                LogHelper.WriteLine("发送至高失败。errcode：" + errcode + "，errmsg：" + jsonSendResult["errmsg"]);
            }
        }
        #endregion
        #endregion

        #region 任务自动分配（已废弃）
        /// <summary>
        /// 任务自动分配
        /// </summary>
        private void TaskAutoAllocation()
        {
            ProjectBLL pBll = new ProjectBLL();

            //新任务分配目录
            string taskAllotmentPath = Convert.ToString(ConfigurationManager.AppSettings["taskAllotmentPath"]);
            //新的学生任务待分配目录
            string studentTaskAllotmentPath = Convert.ToString(ConfigurationManager.AppSettings["studentTaskAllotmentPath"]);
            //所有员工的父级目录
            string employeePath = Convert.ToString(ConfigurationManager.AppSettings["employeePath"]);
            try
            {
                //获取专业类别的ConfigKey和唯一ID
                DataTable dtSpecialtyKey = pBll.GetSpecialtyKey();
                if (dtSpecialtyKey == null)
                {
                    LogHelper.WriteLine("dtSpecialtyKey is null");
                    return;
                }
                //LogHelper.WriteLine("专业类别总数：" + dtSpecialtyKey.Rows.Count);
                //获取尚未分配的任务
                DataTable dtNotAllot = pBll.GetNotCreatedFolderTask(1);
                if (dtNotAllot == null)
                {
                    LogHelper.WriteLine("dtNotAllot is null");
                    return;
                }
                //LogHelper.WriteLine("未分配任务总数:" + dtNotAllot.Rows.Count);
                //所有员工任务数量的目标值
                DataTable dtObjValue = pBll.GetObjectiveValueByEmpNo();
                if (dtObjValue == null)
                {
                    LogHelper.WriteLine("dtObjValue is null");
                    return;
                }
                //LogHelper.WriteLine("任务目标值表中的记录总数：" + dtObjValue.Rows.Count);

                //最外层循环，遍历所有的专业，然后取出该专业下所有任务、该专业下的所有员工，以此进行任务的分配
                for (int indexSpc = 0; indexSpc < dtSpecialtyKey.Rows.Count; indexSpc++)
                {
                    //筛选出 未分配任务中 专业类别与当前正在遍历的专业类别相同 的任务
                    string taskSelectFilter = string.Format("specialtycategory = '{0}'", dtSpecialtyKey.Rows[indexSpc]["configkey"]);
                    DataRow[] drNotAllotWhereSpecialty = dtNotAllot.Select(taskSelectFilter);
                    //遍历当前专业下的所有任务
                    for (int indexCurrentSpc /*当前专业索引*/ = 0; indexCurrentSpc < drNotAllotWhereSpecialty.Length; indexCurrentSpc++)
                    {
                        #region 遍历所有任务待分配的目录，将符合条件的任务目录名添加到list列表中
                        //符合当前专业类别的任务列表
                        ArrayList arrlstAllot = new ArrayList(0);
                        //LogHelper.WriteLine("集合初始化后的容量是："+arrlstAllot.Capacity);

                        //判断新任务分配目录是否存在
                        if (Directory.Exists(taskAllotmentPath))
                        {
                            //任务分配目录
                            DirectoryInfo dirAllotmentPath = new DirectoryInfo(taskAllotmentPath);
                            //遍历新任务待分配目录
                            foreach (DirectoryInfo allotmentTask in dirAllotmentPath.GetDirectories())
                            {
                                string taskNo = allotmentTask.Name;
                                //LogHelper.WriteLine("遍历新任务目录，任务目录名：" + taskNo);
                                if (!arrlstAllot.Contains(taskNo))
                                {
                                    //判断当前任务是否属于当前专业类别
                                    string selectFilter = string.Format("specialtycategory = '{0}' and taskNo = '{1}'", dtSpecialtyKey.Rows[indexSpc]["configkey"], taskNo);
                                    DataRow[] drFinal = dtNotAllot.Select(selectFilter);
                                    //属于当前类别的，才加入list中
                                    if (drFinal.Length > 0)
                                    {
                                        arrlstAllot.Add(taskNo);
                                        LogHelper.WriteLine("加入了一个新任务到集合中：" + taskNo + "，现在集合的容量是：" + arrlstAllot.Count);
                                    }
                                }
                            }
                        }

                        //判断学生任务待分配目录是否存在（因为这个目录过了毕业季随时可能取消）
                        if (Directory.Exists(studentTaskAllotmentPath))
                        {
                            DirectoryInfo dirStuAllotmentPath = new DirectoryInfo(studentTaskAllotmentPath);
                            foreach (DirectoryInfo stuAllotmentTask in dirStuAllotmentPath.GetDirectories())
                            {
                                string taskNo = stuAllotmentTask.Name;
                                LogHelper.WriteLine("遍历新任务目录，任务目录名：" + taskNo);
                                if (!arrlstAllot.Contains(taskNo))
                                {
                                    //判断当前任务是否属于当前专业类别
                                    string selectFilter = string.Format("specialtycategory = '{0}' and taskNo = '{1}'", dtSpecialtyKey.Rows[indexSpc]["configkey"], taskNo);
                                    DataRow[] drFinal = dtNotAllot.Select(selectFilter);
                                    if (drFinal.Length > 0)
                                    {
                                        arrlstAllot.Add(taskNo);
                                        LogHelper.WriteLine("加入了一个新的学生任务到集合中：" + taskNo + "，现在集合的容量是：" + arrlstAllot.Count);
                                    }
                                }
                            }
                        }
                        #endregion
                        //LogHelper.WriteLine("集合添加完毕，arrlstAllot最终容量(最终总的待分配的任务数是)：" + arrlstAllot.Count);

                        int totalTaskAmount = arrlstAllot.Count; //当前专业总的任务数
                        int totalReceiveEmpsAmount = 0;             //当前专业的员工数（在配置表中配置了该专业员工总数）
                        DataTable dtEmpCurrentSpclty = pBll.GetEmpAmountBySpecialty(Convert.ToString(dtSpecialtyKey.Rows[indexSpc]["ID"]), out totalReceiveEmpsAmount);
                        //LogHelper.WriteLine("当前专业总的任务数："+totalTaskAmount+"，当前专业的总人数：" + totalReceiveEmpsAmount);

                        //总的循环次数：任务总数除以该专业的员工数。例如任务总数为9，该专业员工为3人，那么循环次数就是3次，然后每次都取出list中的第一个元素分配，就基本保证了分配均匀。
                        int loopTimes = 0;
                        //如果任务总数对该专业员工数取余为0，那么循环次数就正好是二者之商，每位员工分配的任务数一致。
                        if (totalTaskAmount % totalReceiveEmpsAmount == 0)
                        {
                            loopTimes = totalTaskAmount / totalReceiveEmpsAmount;
                        }
                        //如果任务总数对该专业员工数取余不为0，那么循环次数应该是比二者之商多一次，才能将所有待分配的任务分配掉。
                        else
                        {
                            loopTimes = totalTaskAmount / totalReceiveEmpsAmount + 1;
                        }
                        //LogHelper.WriteLine("最外层循环次数：" + loopTimes);
                        for (int iLoops = 0; iLoops < loopTimes; iLoops++)
                        {
                            //所有员工目录的父级目录
                            if (!Directory.Exists(employeePath))
                            {
                                break;
                            }
                            //员工目录的父级目录
                            DirectoryInfo dirEmpRootPath = new DirectoryInfo(employeePath);
                            //遍历每个员工的目录
                            foreach (DirectoryInfo empFolder in dirEmpRootPath.GetDirectories())
                            {
                                //list中没有任务时，退出循环
                                if (arrlstAllot.Count == 0)
                                {
                                    break;
                                }
                                //员工编号，同时也是员工目录名
                                string empNo = empFolder.Name;
                                //获取list中第一个任务的专业类别（每次都取第一个任务分给符合条件的员工，这样一直取下去，最后list中的任务就能分配完毕了）
                                DataRow[] drSpecialty = dtNotAllot.Select(string.Format("TASKNO = '{0}'", arrlstAllot[0].ToString()));
                                string specialty = drSpecialty[0]["SPECIALTYCATEGORY"].ToString();
                                //LogHelper.WriteLine("第一个任务是" + arrlstAllot[0].ToString() + "，专业类别是" + specialty);

                                //看看当前员工是不是符合该专业
                                DataRow[] drEmpCurrentSpclty = dtEmpCurrentSpclty.Select(string.Format("employeeno = '{0}'", empNo));
                                //LogHelper.WriteLine("drEmpCurrentSpclty.Length：" + drEmpCurrentSpclty.Length);
                                //LogHelper.WriteLine("empNo：" + empNo);
                                //如果不是的，则进入下一位员工
                                if (drEmpCurrentSpclty.Length == 0)
                                {
                                    continue;
                                }

                                //★这里貌似不需要了★******************************************************
                                DataTable dtSpc = pBll.GetSpecialtyBySpcIdAndEmpNo(specialty, empNo);
                                if (dtSpc == null || dtSpc.Rows.Count == 0)
                                {
                                    LogHelper.WriteLine("dtSpc.Rows.Count：" + dtSpc.Rows.Count);
                                    continue;
                                }
                                //判断是否超出分配给该员工的任务数目标值，如果超出，则执行下一位员工
                                int objValue = 0;   //任务数目标值
                                int d_value = 0;    //任务目标值的容错值
                                DataRow[] drObjValue = dtObjValue.Select(string.Format("EMPLOYEENO = '{0}'", empFolder.Name));
                                //LogHelper.WriteLine("drObjValue长度：" + drObjValue.Length);
                                if (drObjValue.Length == 0)
                                {
                                    continue;
                                }
                                objValue = Convert.ToInt32(drObjValue[0]["OBJECTIVEVALUE"]);
                                d_value = Convert.ToInt32(drObjValue[0]["D_VALUE"]);
                                //LogHelper.WriteLine("目标值：" + objValue + "   容错值：" + d_value);
                                objValue = objValue + d_value;          //目标值修正为目标值与容错值之和，因为员工目录下会有因退款、未及时写完成等因素造成的任务完成状态未改为已完成而实际上任务已完成的情况。
                                //获取当前员工积压的任务数
                                int currentTaskAmount = pBll.GetEmployeeCurrentTaskAmount(empNo);
                                //LogHelper.WriteLine("当前员工手里积压的任务数：" + currentTaskAmount);
                                //如果当前积压的任务数小于目标值，则为其分配任务
                                if (currentTaskAmount < objValue)
                                {
                                    //LogHelper.WriteLine("当前积压的任务数小于目标值，则为其分配任务");
                                    string taskNo = arrlstAllot[0].ToString();
                                    string fullTaskFolderName = string.Empty;
                                    //如果当前任务是新任务待分配下的任务，则前面的路径是新任务待分配的路径：taskAllotmentPath
                                    if (Directory.Exists(taskAllotmentPath + "\\" + taskNo))
                                    {
                                        fullTaskFolderName = taskAllotmentPath + "\\" + taskNo;
                                    }
                                    //如果当前任务是学生任务待分配下的任务，则前面的路径是学生任务待分配的路径：studentTaskAllotmentPath
                                    else if (Directory.Exists(studentTaskAllotmentPath + "\\" + taskNo))
                                    {
                                        fullTaskFolderName = studentTaskAllotmentPath + "\\" + taskNo;
                                    }
                                    string fullEmpTaskName = empFolder.FullName + "\\" + taskNo;
                                    //LogHelper.WriteLine("fullEmpTaskName:" + fullEmpTaskName);
                                    //移动目录
                                    MoveFolder(fullTaskFolderName, fullEmpTaskName);
                                    LogHelper.WriteLine("移动完成");
                                    //设置任务的完成人
                                    string projectId = pBll.GetProjectIDByTaskNo(taskNo);
                                    bool isSetFnsPerson2 = pBll.IsSetFinishedPerson2(projectId, string.Empty);
                                    if (!isSetFnsPerson2)
                                    {
                                        bool setSuccess = pBll.SetFinishedPerson2(projectId, empNo);
                                        if (setSuccess)
                                        {
                                            LogHelper.WriteLine(string.Format("设置了任务[{0}]的完成人为[{1}]", taskNo, empNo));
                                            string enteringPerson = drNotAllotWhereSpecialty[indexCurrentSpc]["enteringPerson"].ToString();
                                            if (!trBll.IsExist(enteringPerson, taskNo, null, "0", "2"))
                                            {
                                                int addTaskRemindingFlag = trBll.Add(empNo, enteringPerson, taskNo, null, "0", DateTime.Now.ToString(), null, "0", "0", "2");
                                                if (addTaskRemindingFlag > 0)
                                                {
                                                    LogHelper.WriteLine("成功添加任务[" + taskNo + "]分配给完成人[" + empNo + "]的提醒");
                                                }
                                            }

                                            //添加日志
                                            SystemLog log = new SystemLog();
                                            log.ID = Guid.NewGuid().ToString("N");
                                            log.EMPLOYEEID = "66e15576b4a141f8903655256ab6a254";
                                            log.IPADDRESS = "127.0.0.1";
                                            log.OPERATETYPE = "1";
                                            log.OPERATECONTENT = string.Format("系统分配[{0}]给[{1}]", taskNo, empNo);
                                            log.CREATEDATE = DateTime.Now;
                                            SystemLogBLL slBll = new SystemLogBLL();
                                            bool flag = slBll.Add(log);
                                            if (!flag)
                                            {
                                                LogHelper.WriteLine("系统自动分配日志添加失败");
                                            }
                                        }
                                    }
                                    //移除list中的第一个元素
                                    arrlstAllot.RemoveAt(0);
                                }
                            }
                        }
                        arrlstAllot = null;
                    }
                }
                dtObjValue = null;
                dtNotAllot = null;
                dtSpecialtyKey = null;
            }
            catch (Exception e)
            {
                LogHelper.WriteLine(e.Message + e.StackTrace);
            }

            #region 注释
            //DataTable dsProjectNoAllot = pBll.GetProject("");
            //LogHelper.WriteLine("监测任务目录是否创建");

            ////集合的键索引
            //int folderIndex = 0;

            //ArrayList arrlstAllot = new ArrayList();

            //if (Directory.Exists(taskAllotmentPath))
            //{
            //    //任务分配目录
            //    DirectoryInfo dirAllotmentPath = new DirectoryInfo(taskAllotmentPath);
            //    //遍历新任务待分配目录
            //    foreach (DirectoryInfo allotmentTask in dirAllotmentPath.GetDirectories())
            //    {
            //        if (!dicTasks.ContainsKey(folderIndex.ToString()))
            //        {
            //            dicTasks.Add(folderIndex.ToString(), allotmentTask.Name);
            //            arrlstAllot.Add(allotmentTask.Name);//********************************************************
            //            folderIndex++;
            //        }
            //    }
            //}

            ////新的学生任务待分配目录
            //string studentTaskAllotmentPath = ConfigurationManager.AppSettings["studentTaskAllotmentPath"].ToString();//学生任务分配目录
            ////判断学生任务待分配目录是否存在（因为这个目录随时可能取消）
            //if (Directory.Exists(studentTaskAllotmentPath))
            //{
            //    DirectoryInfo dirStuAllotmentPath = new DirectoryInfo(studentTaskAllotmentPath);
            //    foreach (DirectoryInfo stuAllotmentTask in dirStuAllotmentPath.GetDirectories())
            //    {
            //        if (!dicTasks.ContainsKey(folderIndex.ToString()))
            //        {
            //            dicTasks.Add(folderIndex.ToString(), stuAllotmentTask.Name);
            //            arrlstAllot.Add(stuAllotmentTask.Name);//********************************************************
            //            folderIndex++;
            //        }
            //    }
            //}
            //for (int i = 0; i < dtNotAllot.Rows.Count; i++)
            //{
            //    foreach (KeyValuePair<string, string> kv in dicTasks)
            //    {
            //        string taskNo = dtNotAllot.Rows[i]["TASKNO"].ToString();
            //        string specialty = dtNotAllot.Rows[i]["SPECIALTYCATEGORY"].ToString();//专业类别，这里是专业类别的代码（在配置表里的key），如1、2、3，并非是ID
            //        if (kv.Value.StartsWith(taskNo))
            //        {
            //            DirectoryInfo dirEmpFolders = new DirectoryInfo(employeePath);
            //            foreach (DirectoryInfo empFolder in dirEmpFolders.GetDirectories())
            //            {
            //                DataRow[] dr = null;

            //            }
            //        }
            //    }
            //}

            ////总任务数除以被分配的人数  为   总的循环次数
            //string[] arr = { "20170001", "20170002", "20170003", "20170004" };
            ////ArrayList arrlstAllot = new ArrayList(arr);
            //int totalTaskAmount = arrlstAllot.Capacity;
            //int totalReceiveEmpsAmount = 4;//待分配的员工数
            //int loopTimes = 0;
            //if (totalTaskAmount % totalReceiveEmpsAmount == 0)
            //{
            //    loopTimes = totalTaskAmount / totalReceiveEmpsAmount;
            //}
            //else
            //{
            //    loopTimes = totalTaskAmount / totalReceiveEmpsAmount + 1;
            //}
            //DataTable dtObjValue = pBll.GetObjectiveValueByEmpNo();
            //for (int i = 0; i < loopTimes; i++)
            //{
            //    foreach (DirectoryInfo empFolder in new DirectoryInfo(employeePath).GetDirectories())
            //    {
            //        //如果当前员工目录是需要自动分配的员工的目录
            //        DataRow[] drSpecialty=dtNotAllot.Select(string.Format("TASKNO = '{0}'", arrlstAllot[0].ToString()));
            //        string specialty=drSpecialty[0]["SPECIALTY"].ToString();

            //        if ("name".Contains(empFolder.Name))
            //        {
            //            string empNo = empFolder.Name;
            //            DataTable dtSpc = pBll.GetSpecialtyBySpcIdAndEmpNo(specialty, empNo);
            //            if (dtSpc == null || dtSpc.Rows.Count == 0)
            //            {
            //                continue;
            //            }
            //            //判断是否超出分配给该员工的任务数目标值，如果超出，则执行下一位员工
            //            int objValue = 0;
            //            int d_value = 0;
            //            DataRow[] drObjValue = dtObjValue.Select(string.Format("EMPLOYEENO = '{0}'", empFolder.Name));
            //            if (drObjValue.Length == 0)
            //            {
            //                continue;
            //            }
            //            objValue = Convert.ToInt32(drObjValue[0]["OBJECTIVEVALUE"]);
            //            d_value = Convert.ToInt32(drObjValue[0]["D_VALUE"]);
            //            objValue = objValue + d_value;//*************************************************
            //            //获取当前员工积压的任务数
            //            int currentTaskAmount = pBll.GetEmployeeCurrentTaskAmount(empFolder.Name);
            //            //如果当前积压的任务数小于目标值，则为其分配任务
            //            if (currentTaskAmount < objValue)
            //            {
            //                string taskNo = arrlstAllot[0].ToString();
            //                string fullTaskFolderName = string.Empty;
            //                if (Directory.Exists(taskAllotmentPath + taskNo))
            //                {
            //                    fullTaskFolderName = taskAllotmentPath + taskNo;
            //                }
            //                else if (Directory.Exists(studentTaskAllotmentPath + taskNo))
            //                {
            //                    fullTaskFolderName = studentTaskAllotmentPath + taskNo;
            //                }
            //                string fullEmpTaskName = empFolder.FullName + taskNo;
            //                //移动目录
            //                MoveFolder(fullTaskFolderName, fullEmpTaskName);
            //                //设置任务的完成人
            //                string projectId = pBll.GetProjectIDByTaskNo(taskNo);
            //                bool isSetFnsPerson2 = pBll.IsSetFinishedPerson2(projectId, empNo);
            //                if (!isSetFnsPerson2)
            //                {
            //                    bool setSuccess = pBll.SetFinishedPerson2(projectId, empNo);
            //                    if (setSuccess)
            //                    {
            //                        LogHelper.WriteLine(string.Format("设置了任务[{0}]的完成人为[{1}]", taskNo, empNo));
            //                        if (!trBll.IsExist("enteringPerson", taskNo, "0", "2"))
            //                        {
            //                            int addTaskRemindingFlag = trBll.Add(empNo, "enteringPerson", taskNo, null, "0", DateTime.Now.ToString(), null, "0", "0", "2");
            //                            if (addTaskRemindingFlag > 0)
            //                            {
            //                                LogHelper.WriteLine("成功添加任务[" + taskNo + "]分配给完成人[" + empNo + "]的提醒");
            //                            }
            //                        }
            //                    }
            //                }
            //                arrlstAllot.RemoveAt(0);
            //                //continue;
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// 移动目录
        /// </summary>
        /// <param name="sourceDirName">源路径</param>
        /// <param name="destDirName">目标路径</param>
        protected void MoveFolder(string sourceDirName, string destDirName)
        {
            if (Directory.Exists(sourceDirName))
            {
                Directory.Move(sourceDirName, destDirName);
            }
        }
        #endregion

        #region Remarks
        /*
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
                                        trBll.Add(empNo, string.Empty, direcChild.Name + modifyRecord.Name, "0", DateTime.Now.ToString(), null, isFinished0 ? "1" : "0", "1");
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

                                trBll.Add(empNo, enteringPerson, direcChild.Name, "0", DateTime.Now.ToString(), expireDate, isFinished, taskType);
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
         */
        #endregion

        #region 辅助方法
        /// <summary>
        /// 转化为时间
        /// </summary>
        /// <param name="inputDate">输入的时间</param>
        /// <param name="returnDate">返回的时间</param>
        /// <returns>是否成功</returns>
        private bool ConvertToDateTime(string inputDate, out string returnDate)
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

        /// <summary>
        /// 设置工作内存占用
        /// </summary>
        /// <param name="maxWorkingSet"></param>
        public static void SetWorkingSet(int maxWorkingSet)
        {
            System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)maxWorkingSet;
        }
        #endregion
    }
}
