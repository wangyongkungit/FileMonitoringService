using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringDAL
{
    public class ProjectDAL
    {
        /// <summary>
        /// 获得所有任务
        /// </summary>
        /// <returns></returns>
        public DataTable GetProject()
        {
            string sql = "SELECT ID,taskno,isfinished,finishedperson from project";
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 是否已经设置了完成人
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userNo">已经设置完成人的情况下，返回完成人的EMPLOYEENO</param>
        /// <returns></returns>
        public bool IsSetFinishedPerson(string projectID, out string userNo)
        {
            userNo = string.Empty;
            string sql = string.Format(@"SELECT EMP.EMPLOYEENO FROM PROJECT P INNER JOIN EMPLOYEE EMP ON P.FINISHEDPERSON=EMP.ID WHERE P.ID='{0}'", projectID);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                userNo = ds.Tables[0].Rows[0][0].ToString();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置完成人
        /// </summary>
        /// <param name="userNo">员工编号</param>
        /// <param name="projectID">工程ID</param>
        /// <returns></returns>
        public bool UpdateFinishedPerson(string userNo, string projectID)
        {
            string sql = string.Format(@"update project set finishedperson=(select id from employee where employeeno ='{0}') WHERE id='{1}'", userNo, projectID);
            int r = MySqlHelper.ExecuteNonQuery(sql);
            return r > 0;
        }

        /// <summary>
        /// 修改完成状态
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool UpdateFinishedStatus(string projectID)
        {
            string sql = string.Format(@"update project set isfinished='1' where ID='{0}'", projectID);
            int r = MySqlHelper.ExecuteNonQuery(sql);
            return r > 0;
        }

        /// <summary>
        /// 判断当前任务的修改记录是否存在
        /// </summary>
        /// <param name="taskNoOriginal">任务的原始文件夹名</param>
        /// <param name="modifyFolderName">修改文件夹名</param>
        /// <returns></returns>
        public bool IsExistProjectModify(string taskNoOriginal, string modifyFolderName)
        {
            string sql = string.Format(@"SELECT * from project p
                            INNER join projectmodify pm
                            on p.taskno='{0}'
                            and pm.foldername='{1}'", taskNoOriginal, modifyFolderName);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据任务编号判断任务是否存在于数据库中
        /// </summary>
        /// <param name="taskNo"></param>
        /// <returns></returns>
        public bool IsExistProjectByTaskNo(string taskNo)
        {
            string sql = string.Format("SELECT COUNT(*) FROM PROJECT WHERE TASKNO='{0}'", taskNo);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0;
            }
            return false;
        }

        /// <summary>
        /// 添加修改记录
        /// </summary>
        /// <param name="taskNoOriginal">任务原始文件名</param>
        /// <param name="modifyFolderName">修改记录文件夹名</param>
        /// <returns></returns>
        public bool AddProjectModify(string taskNoOriginal, string modifyFolderName)
        {
            string sql = string.Format(@"INSERT INTO projectmodify ( ID ,PROJECTID ,FOLDERNAME  )
                                VALUES ('{0}',(SELECT id from project where taskno='{1}'),'{2}')",
                                Guid.NewGuid(), taskNoOriginal, modifyFolderName);
            int r = MySqlHelper.ExecuteNonQuery(sql);
            return r > 0;
        }

        /// <summary>
        /// 修改售后的完成状态
        /// </summary>
        /// <param name="taskNoOriginal"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool UpdateProjectModifyFinished(string taskNoOriginal, string modifyFolderName)
        {
            string sql = string.Format(@"UPDATE projectmodify set isfinished=1 
                            where projectID=(select id from project where taskNo='{0}')
                            and folderName='{1}'",
                            taskNoOriginal, modifyFolderName);
            int r = MySqlHelper.ExecuteNonQuery(sql);
            return r > 0;
        }
    }
}
