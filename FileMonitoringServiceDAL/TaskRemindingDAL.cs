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
        /// 判断任务完成稿是否提醒，在用
        /// </summary>
        /// <param name="userIDorEmpNo">userID</param>
        /// <param name="folder">目录名（即任务编号）</param>
        /// <param name="modifyFolder">修改任务名</param>
        /// <param name="taskType">任务类型：0，新任务；1，售后；2，倒计时1小时；3，新任务待分配</param>
        /// <param name="toUserType">发送到的用户的类型</param>
        /// <returns></returns>
        public bool IsExist(string userIDorEmpNo, string folder, string modifyFolder, string taskType, string toUserType)
        {
            string sql = string.Empty;
            //1是客服人员
            if (toUserType == "1")
            {
                sql = string.Format(@"SELECT count(*) from taskreminding where enteringperson = '{0}' and folder = '{1}'  and taskType = '{2}' AND TOUSERTYPE = {3}", userIDorEmpNo, folder, taskType, toUserType);
                //是修改任务的
                if (!string.IsNullOrEmpty(modifyFolder))
                {
                    sql += string.Format(" and modifyFolder = '{0}'", modifyFolder);
                }
            }
            else if (toUserType == "2")  //造价员
            {
                sql = string.Format(@"SELECT count(*) from taskreminding where employeeNo = '{0}' and folder = '{1}' and taskType = '{2}' AND TOUSERTYPE = {3}", userIDorEmpNo, folder, taskType, toUserType);
                //是修改任务的
                if (!string.IsNullOrEmpty(modifyFolder))
                {
                    sql += string.Format(" and modifyFolder = '{0}'", modifyFolder);
                }
            }
            else if (toUserType == "0") //管理员
            {
                sql = string.Format(@"SELECT COUNT(*) FROM TASKREMINDING WHERE FOLDER = '{0}' AND TOUSERTYPE = '{1}'", folder, toUserType);
            }
            else
            {
                return false;
            }
            DataSet ds = MySqlHelper.GetDataSet(sql);
            int amount = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                amount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return amount > 0;
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
        public int Add(string userNO, string enteringPerson, string folder, string modifyFolder, string isReminded, string createDate, string expireDate, string isFinished, string taskType,string toUserType)
        {
            modifyFolder = modifyFolder == null ? "null" : string.Format("'{0}'", modifyFolder);
            string insertSql = string.Format(@"insert into taskreminding(ID,EMPLOYEENO,ENTERINGPERSON,FOLDER,MODIFYFOLDER,ISREMINDED,CREATEDATE,EXPIREDATE,ISFINISHED,TASKTYPE,TOUSERTYPE)
                                              values('{0}','{1}','{2}','{3}', {4}, '{5}', '{6}', {7}, '{8}', '{9}', '{10}')",
                                             Guid.NewGuid(), userNO, enteringPerson, folder, modifyFolder, isReminded, createDate, expireDate == null ? "null" : string.Format("'{0}'", expireDate), isFinished, taskType, toUserType);
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
