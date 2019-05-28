using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringDAL
{
    /// <summary>
    /// 任务分配想关DAL
    /// </summary>
    public class TaskAssignConfigDetailsDAL
    {
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,EMPLOYEEID,SPECIALTYCATEGORY,QUALITYSCORE,AVAILABLE,TIMEMULTIPLE,SPECIALTYTYPE ");
            strSql.Append(" FROM taskassignconfigdetails ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetToAllotEmployeesSpecialties()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select tacd.ID,EMPLOYEEID,SPECIALTYCATEGORY,QUALITYSCORE,AVAILABLE,TIMEMULTIPLE,SPECIALTYTYPE,cv.CONFIGVALUE SPECIALTYNAME
                            FROM taskassignconfigdetails tacd
                            INNER JOIN configvalue cv
                            ON tacd.SPECIALTYCATEGORY = cv.CONFIGKEY
                            AND cv.CONFIGTYPEID = 'b47d2587-6421-4dc5-b0be-7ce595d6bdc0'
                            WHERE employeeId in (SELECT EMPLOYEEID FROM taskassignconfig WHERE AVAILABLE = 1)
                            AND available = 1 and specialtytype = 0");
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取可分配任务的所有员工
        /// </summary>
        /// <returns></returns>
        public DataSet GetCanAssignEmployees(string orderBy)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,EMPLOYEENO,`NAME`, e.DINGTALKUSERID FROM employee e  where AVAILABLE = 1 and ID IN (SELECT employeeId FROM taskassignconfig)");
            if (!string.IsNullOrEmpty(orderBy))
            {
                strSql.Append(" ORDER BY " + orderBy);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取所有未完成的任务
        /// </summary>
        /// <returns></returns>
        public DataSet GetNotFinishProjects()
        {
            string strSql = @"SELECT p.Id prjId, ps.finishedPerson, p.WANGWANGNAME, p.timeNeeded, p.isFinished  FROM project p
                             INNER JOIN projectsharing ps
                             ON p.ID = ps.projectID
                             WHERE (P.isFinished <> 1 OR p.isFinished IS NULL)";
            return DbHelperMySQL.Query(strSql);
        }

        /// <summary>
        /// 获取时间充足的员工此前做过的任务
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public DataSet GetPreviousTask(string employees)
        {
            string strSql = @"SELECT p.WANGWANGNAME, ps.FINISHEDPERSON FROM project p INNER JOIN projectsharing ps on p.ID = ps.PROJECTID 
                            WHERE ps.FINISHEDPERSON in (" + employees + ")";
            return DbHelperMySQL.Query(strSql);
        }

        /// <summary>
        /// 获取所有的专业质量分
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllTaskAssignDetails()
        {
            string strSql = @"SELECT spc.*, IFNULL(tacd.EMPLOYEEID,'') EMPLOYEEID , IFNULL(tacd.ID,'') ID, IFNULL(tacd.QUALITYSCORE,0) QUALITYSCORE, IFNULL(tacd.SPECIALTYCATEGORY,'') SPECIALTYCATEGORY,
                             IFNULL(tacd.AVAILABLE,0) AVAILABLE, IFNULL(tacd.TIMEMULTIPLE,1) TIMEMULTIPLE
                             FROM (SELECT cv.configvalue specialtyName, cv.configKey specialtyKey from configvalue cv
                             INNER JOIN configtype ct
                             on cv.CONFIGTYPEID = ct.CONFIGTYPEID
                             WHERE ct.CONFIGTYPENAME = '专业类别') spc
                             LEFT JOIN taskassignconfigdetails tacd
                             ON spc.specialtyKey = tacd.SPECIALTYCATEGORY
                             AND tacd.specialtyType = '0'";
            return DbHelperMySQL.Query(strSql);
        }

        /// <summary>
        /// 获取指定的几位员工的预期目标工资
        /// </summary>
        /// <param name="employeeIDs">员工ID集</param>
        /// <returns></returns>
        public DataSet GetAllCurrentAmount(string employeeIDs)
        {
            string strSql = @"SELECT p.ID prjId, p.TASKNO, IFNULL(p.ORDERAMOUNT,0) ORDERAMOUNT, IFNULL(ps.PROPORTION,1) PROPORTION, ps.FINISHEDPERSON FROM project p
                             INNER JOIN projectsharing ps
                             ON p.id = ps.PROJECTID
                             AND ps.FINISHEDPERSON IN (" + employeeIDs + @")
                             WHERE DATE_FORMAT(p.ORDERDATE,'%Y-%m') = DATE_FORMAT(NOW(),'%Y-%m')";
            return DbHelperMySQL.Query(strSql);
        }
    }
}
