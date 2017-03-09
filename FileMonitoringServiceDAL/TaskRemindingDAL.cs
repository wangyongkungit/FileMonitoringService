using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringDAL
{
    public class TaskRemindingDAL
    {
        public bool IsExist(string userNo, string folder)
        {
            string strSql = string.Format(@"select * from taskreminding where employeeno='{0}' and folder='{1}' and isreminded='0'", userNo, folder);
            DataSet ds = MySqlHelper.GetDataSet(strSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public int GetTaskAmount(string userNo, string taskType)
        {
            string strSql = string.Format(@"select count(*) from taskreminding where employeeno='{0}' and tasktype='{1}'", userNo, taskType);
            DataSet ds = MySqlHelper.GetDataSet(strSql);
            int amount = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                amount = Convert.ToInt32( ds.Tables[0].Rows[0][0]);
            }
            return amount;
        }

        public int GetTaskAmount(string userNo, string taskType, string taskName)
        {
            string strSql = string.Format(@"select count(*) from taskreminding where employeeno='{0}' and tasktype='{1}' and folder='{2}'", userNo, taskType, taskName);
            DataSet ds = MySqlHelper.GetDataSet(strSql);
            int amount = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                amount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return amount;
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
            string insertSql = string.Format(@"insert into taskreminding(ID,EMPLOYEENO,FOLDER,ISREMINDED,CREATEDATE,EXPIREDATE,ISFINISHED,TASKTYPE)
                                              values('{0}','{1}','{2}','{3}','{4}', {5}, '{6}', '{7}')",
                                             Guid.NewGuid(), userNO, folder, isReminded, createDate, expireDate == null ? "null" : string.Format("'{0}'", expireDate), isFinished, taskType);
            int rows = MySqlHelper.ExecuteNonQuery(insertSql);
            return rows;
        }

        /// <summary>
        /// 修改完成状态
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="folder">文件夹名</param>
        /// <returns></returns>
        public int SetIsFinished(string userNo, string folder)
        {
            string updateSql = string.Format(@"update taskreminding set isfinished='1' where employeeno='{1}' and folder='{2}'", userNo, folder);
            int rows = MySqlHelper.ExecuteNonQuery(updateSql);
            return rows;
        }

        /// <summary>
        /// 删除TaskReminding
        /// </summary>
        /// <param name="userNO"></param>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public bool Delete(string userNO, string taskType)
        {
            string deleteSql = string.Format(@"delete from taskreminding where employeeno='{0}' and tasktype='{1}'", userNO, taskType);
            int rows = MySqlHelper.ExecuteNonQuery(deleteSql);
            return rows > 0;
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
            string deleteSql = string.Format(@"delete from taskreminding where employeeno='{0}' and tasktype='{1}' and folder='{2}'", userNO, taskType, taskName);
            int rows = MySqlHelper.ExecuteNonQuery(deleteSql);
            return rows > 0;
        }

        /// <summary>
        /// 获取提醒是否存在
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool RemindIsExist(string userNo, string type)
        {
            string strSql = string.Format(@"select * from remindneed where employeeno='{0}' and isneedremind='1' and remindtype='{1}'", userNo, type);
            DataSet ds = MySqlHelper.GetDataSet(strSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加提醒
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="type">任务类型</param>
        /// <returns></returns>
        public bool AddRemind(string userNo, string type)
        {
            string insertSql = String.Format(@"insert into remindneed(ID,EMPLOYEENO,ISNEEDREMIND,REMINDTYPE) VALUES ('{0}','{1}','{2}','{3}')",
                                Guid.NewGuid(), userNo, "1", type);
            int rows = MySqlHelper.ExecuteNonQuery(insertSql);
            return rows > 0;
        }

        /// <summary>
        /// 修改提醒状态（即删除记录，这样就不会提醒了）
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="type">任务类型</param>
        /// <returns></returns>
        public bool UpdateRemind(string userNo, string type)
        {
            string updateSql = string.Format(@"delete from remindneed where employeeno='{0}' and remindtype='{1}'", userNo, type);
            int rows = MySqlHelper.ExecuteNonQuery(updateSql);
            return rows > 0;
        }
    }
}
