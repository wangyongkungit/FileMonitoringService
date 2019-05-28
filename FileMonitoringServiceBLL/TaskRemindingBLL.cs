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
        /// 判断是否已添加任务提醒，在用
        /// </summary>
        /// <param name="enteringPerson">录入人ID</param>
        /// <param name="folder">目录名（即任务编号）</param>
        /// <param name="modifyFolder">修改任务名</param>
        /// <param name="taskType">任务类型：0，新任务；1，售后；2，倒计时3小时；3，新任务待分配</param>
        /// <param name="toUserType">发送到的用户的类型</param>
        /// <returns></returns>
        public bool IsExist(string enteringPerson, string folder, string modifyFolder, string taskType, string toUserType)
        {
            return trDal.IsExist(enteringPerson, folder, modifyFolder, taskType, toUserType);
        }

        /// <summary>
        /// 添加TaskReminding
        /// </summary>
        /// <param name="userNO">员工编号</param>
        /// <param name="enteringPerson">录入人</param>
        /// <param name="folder">任务编号（即任务目录名）</param>
        /// <param name="modifyFolder"></param>
        /// <param name="isReminded">是否已提醒，新增时固定为0，表示未提醒</param>
        /// <param name="createDate">创建时间</param>
        /// <param name="expireDate">截止时间</param>
        /// <param name="isFinished">是否完成</param>
        /// <param name="taskType">任务类型</param>
        /// <param name="toUserType">需发送到的用户类型</param>
        /// <returns></returns>
        public int Add(string userNO, string enteringPerson, string folder, string modifyFolder, string isReminded, string createDate, string expireDate, string isFinished, string taskType, string toUserType)
        {
            return trDal.Add(userNO, enteringPerson, folder, modifyFolder, isReminded, createDate, expireDate, isFinished, taskType, toUserType);
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
