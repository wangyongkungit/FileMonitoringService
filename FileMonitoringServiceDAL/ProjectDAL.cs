using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
        public DataTable GetProject(string where)
        {
            string sql = "SELECT ID,taskno,EXPIREDATE,isfinished,finishedperson,enteringperson,orderAmount from project";
            if (!string.IsNullOrEmpty(where))
            {
                sql += where;
            }
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 根据任务编号获得任务ID
        /// <param name="taskNo"></param>
        /// </summary>
        /// <returns></returns>
        public string GetProjectIDByTaskNo(string taskNo)
        {
            string sql = string.Format("SELECT ID from project WHERE TASKNO = '{0}'", taskNo);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToString(ds.Tables[0].Rows[0]["ID"]);
            }
            return string.Empty;
        }

        public DataTable GetProjectNoAllot(string where)
        {
            string sql = @"";
            return null;
        }

        /*
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
        } */

        /// <summary>
        /// 获取完成人的数量
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="employeeNo">员工编号</param>
        /// <returns>返回完成人的数量</returns>
        public bool IsSetFinishedPerson2(string projectID, string employeeNo)
        {
            string sql = string.Format(@"SELECT COUNT(*) FROM projectsharing WHERE PROJECTID = '{0}'
                            and finishedperson in (SELECT ID FROM employee WHERE employeeNO = '{1}')", projectID, employeeNo);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            int amount = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                amount = Convert.ToInt16(ds.Tables[0].Rows[0][0]);
            }
            return amount > 0;
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
        /// 设置完成人（新）
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool SetFinishedPerson2(string projectID, string empNo)
        {
            // 2018-06-24，设置完成人时，需要将之前的完成人清空。因为任务现在调整为只有一个完成人。
            string delSql = string.Format("DELETE FROM projectsharing WHERE PROJECTID = '{0}'", projectID);
            MySqlHelper.ExecuteNonQuery(delSql);

            string sql = string.Format(@"INSERT INTO projectsharing ( ID ,PROJECTID ,FINISHEDPERSON ,PROPORTION ,CREATEDATE )
                             VALUES ('{0}','{1}',(SELECT ID FROM employee WHERE employeeNO = '{2}'),null,'{3}')",
                                       Guid.NewGuid().ToString(), projectID, empNo, DateTime.Now);
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
        /// 判断新的修改任务是否存在
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool IsExistNewModifyScript(string projectID, string modifyFolderName)
        {
            string sql = string.Format("SELECT count(*) from projectmodify WHERE projectid='{0}' AND foldername='{1}' AND ( isfinished = 0 OR isfinished IS NULL )", projectID, modifyFolderName);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0;
            }
            return false;
        }

        /// <summary>
        /// 判断完成稿是否存在
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="modifyFolderName"></param>
        /// <returns></returns>
        public bool IsExistFinalModifyScript(string projectID, string modifyFolderName)
        {
            string sql = string.Format("SELECT count(*) from projectmodify WHERE projectid = '{0}' AND foldername = '{1}' AND isfinished = 1", projectID, modifyFolderName);
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
        /// <param name="modifyName"></param>
        /// <param name="projectModifyID"></param>
        /// <param name="prjMdfFolder"></param>
        /// <param name="reviewStatus"></param>
        /// <returns></returns>
        public bool IsExistModifyTask(string taskNo, string modifyName, out string projectModifyID, /*out string prjMdfFolder,*/ out int reviewStatus)
        {
            projectModifyID = string.Empty;
            //prjMdfFolder = string.Empty;
            reviewStatus = 0;
            string sql = string.Format(@"SELECT p.id, pm.id PID, pm.foldername, PM.REVIEWSTATUS FROM project p
                         INNER JOIN projectmodify pm
                         ON p.id = pm.projectid
                         WHERE p.TASKNO='{0}' AND FOLDERNAME = '{1}'", taskNo, modifyName);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                projectModifyID = ds.Tables[0].Rows[0]["PID"].ToString();
                //prjMdfFolder = ds.Tables[0].Rows[0]["FOLDERNAME"].ToString();
                reviewStatus = Convert.ToInt16(ds.Tables[0].Rows[0]["REVIEWSTATUS"]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取尚未分配完成人并且资料已经上传完成的任务
        /// </summary>
        /// <returns></returns>
        public DataTable GetNotCreatedFolderTask(int isCreatedFolder)
        {
            string sql = @"SELECT p.ID,p.TASKNO,P.SPECIALTYCATEGORY,P.EXPIREDATE,p.enteringPerson,p.timeneeded,p.WANGWANGNAME FROM project p WHERE id NOT IN (SELECT PROJECTID FROM projectsharing)
                            AND P.MATERIALISUPLOAD = 1 AND ISCREATEDFOLDER = " + isCreatedFolder.ToString() + " AND ISDELETED = 0"; //资料必须已经上传完成
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 设置“是否任务目录”值为已创建
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool SetIsCreatedFolder(string projectID)
        {
            string sql = string.Format(@"UPDATE PROJECT SET ISCREATEDFOLDER = 1 WHERE ID = '{0}'", projectID);
            int r = MySqlHelper.ExecuteNonQuery(sql);
            return r > 0;
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

        /// <summary>
        /// 获取专业类别的configkey
        /// </summary>
        /// <returns></returns>
        public DataTable GetSpecialtyKey()
        {
            string sql = string.Format("SELECT DISTINCT(configkey),id from configvalue cv where cv.configtypeid = 'b47d2587-6421-4dc5-b0be-7ce595d6bdc0' ORDER BY configkey");
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 根据员工编号获取其任务目标值
        /// </summary>
        /// <returns></returns>
        public DataTable GetObjectiveValueByEmpNo()
        {
            string sql = @"SELECT objv.id,OBJECTIVEVALUE,D_VALUE,employeeno from taskobjectivevalue objv
                         INNER JOIN employee e
                         on objv.employeeid = e.id";
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前员工尚未完成的任务数（手里有几个活正在做）
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public int GetEmployeeCurrentTaskAmount(string empNo)
        {
            string sql = string.Format(@"SELECT count(*) from project p
                        INNER JOIN projectsharing ps
                        on p.id = ps.PROJECTID
                         WHERE ps.FINISHEDPERSON in(SELECT id from employee where employeeno = '{0}')
                         and p.isfinished = 0 
                         and  DATE_SUB(CURDATE(), INTERVAL 15 DAY) <= date(EXPIREDATE)
                        and TRANSACTIONstatus <>5 ", empNo);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return 0;
        }

        /// <summary>
        /// 根据专业类别码和员工编号获取数据
        /// </summary>
        /// <param name="spcConfigKey"></param>
        /// <param name="employeeNo"></param>
        /// <returns></returns>
        public DataTable GetSpecialtyBySpcIdAndEmpNo(string spcConfigKey, string employeeNo)
        {
            string sql = string.Format(@"SELECT * from employeespecialty
                         WHERE specialtyid in 
                        (SELECT id from configvalue
                         WHERE configtypeid in (SELECT configtypeid from configtype WHERE configtypename = '专业类别') and configkey = {0})
                         and employeeid in (SELECT id from employee WHERE employeeno = '{1}')", spcConfigKey, employeeNo);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前专业的人数
        /// </summary>
        /// <param name="specialtyId">专业ID</param>
        /// <returns></returns>
        public DataTable GetEmpAmountBySpecialty(string specialtyId, out int empAmount)
        {
            empAmount = 0;
            string sql = string.Format(@"SELECT specialtyid,employeeno from employeeSpecialty es
                             INNER JOIN employee e
                             on es.employeeid = e.id WHERE specialtyid = '{0}'", specialtyId);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                empAmount = ds.Tables[0].Rows.Count;
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取任务的要求的交稿时间
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="employeeNo">员工编号</param>
        /// <returns>返回完成人的数量</returns>
        public DataSet GetModifyTaskByPrjIdAndTaskName(string projectID, string taskName)
        {
            string sqlExist = string.Format(@"SELECT count(*) FROM filecategory  WHERE PROJECTID = '{0}' AND PARENTID = '0' AND FOLDERNAME = '{1}' AND CATEGORY = '3'
                                AND ID NOT IN (SELECT PARENTID FROM filecategory WHERE PROJECTID = '{0}')", projectID, taskName);
            DataSet ds01 = MySqlHelper.GetDataSet(sqlExist);
            int cnt = 0;
            if (ds01 != null && ds01.Tables.Count > 0 && ds01.Tables[0].Rows.Count > 0)
            {
                cnt = Convert.ToInt32(ds01.Tables[0].Rows[0][0]);
            }
            if (cnt == 0)
            {
                string sql = string.Format(@"SELECT expireDate from filecategory WHERE projectid='{0}' AND foldername='{1}'",
                                                    projectID, taskName);
                DataSet ds = MySqlHelper.GetDataSet(sql);
                return ds;
            }
            return null;
        }
    }
}