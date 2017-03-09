using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileMonitoringDAL;
using System.Data;

namespace FileMonitoringServiceBLL
{
    public class TaskRemindingBLL
    {
        TaskRemindingDAL trDal = new TaskRemindingDAL();

        public bool IsExist(string userNo, string folder)
        {
            return trDal.IsExist(userNo, folder);
        }

        public int GetTaskAmount(string userNo, string taskType)
        {
            return trDal.GetTaskAmount(userNo, taskType);
        }

        public int GetTaskAmount(string userNo, string taskType, string taskName)
        {
            return trDal.GetTaskAmount(userNo, taskType, taskName);
        }

        /// <summary>
        /// 添加TaskReminding
        /// </summary>
        /// <param name="userNO"></param>
        /// <param name="folder"></param>
        /// <param name="isReminded"></param>
        /// <param name="createDate"></param>
        /// <param name="expireDate"></param>
        /// <param name="isFinished"></param>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public int Add(string userNO, string folder, string isReminded, string createDate, string expireDate, string isFinished, string taskType)
        {
            return trDal.Add(userNO, folder, isReminded, createDate, expireDate, isFinished, taskType);
        }

        /// <summary>
        /// 修改完成状态
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="folder">文件夹名</param>
        /// <returns></returns>
        public int SetIsFinished(string userNo, string folder)
        {
            return trDal.SetIsFinished(userNo, folder);
        }

        /// <summary>
        /// 删除TaskReminding
        /// </summary>
        /// <param name="userNO"></param>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public bool Delete(string userNO, string taskType)
        {
            return trDal.Delete(userNO, taskType);
        }

        /// <summary>
        /// 删除TaskReminding
        /// </summary>
        /// <param name="userNO"></param>
        /// <param name="taskType"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public bool Delete(string userNO, string taskType, string taskName)
        {
            return trDal.Delete(userNO, taskType, taskName);
        }

        /// <summary>
        /// 获取提醒是否存在
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool RemindIsExist(string userNo, string type)
        {
            return trDal.RemindIsExist(userNo, type);
        }

        /// <summary>
        /// 添加提醒
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="type">任务类型</param>
        /// <returns></returns>
        public bool AddRemind(string userNo, string type)
        {
            return trDal.AddRemind(userNo, type);
        }

        /// <summary>
        /// 修改提醒状态（即删除记录，这样就不会提醒了）
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="type">任务类型</param>
        /// <returns></returns>
        public bool UpdateRemind(string userNo, string type)
        {
            return trDal.UpdateRemind(userNo, type);
        }
    }
}
