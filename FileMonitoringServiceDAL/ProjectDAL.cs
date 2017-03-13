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
        /// 判断完成稿是否存在
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool IsExistFinalModifyScript(string projectID, string modifyFolderName)
        {
            string sql = string.Format("SELECT count(*) from projectmodify WHERE projectid='{0}' AND foldername='{1}' AND isfinished=1", projectID, modifyFolderName);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0;
            }
            return false;
        }

        /// <summary>
        /// 添加一条修改任务记录
        /// </summary>
        /// <param name="projectID">对应的普通任务的ID</param>
        /// <param name="folderName">目录名</param>
        /// <param name="isFinished">是否完成</param>
        /// <param name="reviewStatus">是否审核通过</param>
        /// <param name="dtCreate">创建时间</param>
        /// <returns></returns>
        public bool AddProjectModify(string projectID, string folderName, int isFinished, int reviewStatus, DateTime dtCreate)
        {
            string sql = string.Format(@"INSERT INTO projectmodify ( ID ,PROJECTID ,FOLDERNAME ,ISFINISHED ,REVIEWSTATUS ,createdate )
	                                    VALUES ('{0}','{1}','{2}',{3},{4},'{5}')",
                                              Guid.NewGuid(), projectID, folderName, isFinished, reviewStatus, dtCreate);
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

        /// <summary>
        /// 判断修改任务是否存在
        /// </summary>
        /// <param name="taskNo"></param>
        /// <param name="projectModifyID"></param>
        /// <param name="prjMdfFolder"></param>
        /// <param name="reviewStatus"></param>
        /// <returns></returns>
        public bool IsExistModifyTask(string taskNo, out string projectModifyID, out string prjMdfFolder, out int reviewStatus)
        {
            projectModifyID = string.Empty;
            prjMdfFolder = string.Empty;
            reviewStatus = 0;
            string sql = string.Format(@"SELECT p.id,pm.id PID,pm.foldername,PM.REVIEWSTATUS FROM project p
                         INNER JOIN projectmodify pm
                         ON p.id = pm.projectid
                         WHERE p.TASKNO='{0}'", taskNo);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                projectModifyID = ds.Tables[0].Rows[0]["PID"].ToString();
                prjMdfFolder = ds.Tables[0].Rows[0]["FOLDERNAME"].ToString();
                reviewStatus = Convert.ToInt16(ds.Tables[0].Rows[0]["REVIEWSTATUS"]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置审核状态为通过
        /// </summary>
        /// <param name="projectModifyID"></param>
        /// <returns></returns>
        public bool SetReviewPass(string projectModifyID)
        {
            string sql = string.Format("UPDATE projectmodify pm set pm.reviewstatus=1 WHERE pm.id='{0}'", projectModifyID);
            int r = MySqlHelper.ExecuteNonQuery(sql);
            return r > 0;
        }
    }
}
