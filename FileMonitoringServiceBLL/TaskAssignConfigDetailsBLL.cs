using FileMonitoringDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringServiceBLL
{
    public class TaskAssignConfigDetailsBLL
    {
        TaskAssignConfigDetailsDAL tacdDal = new TaskAssignConfigDetailsDAL();
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return tacdDal.GetList(strWhere);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetToAllotEmployeesSpecialties()
        {
            return tacdDal.GetToAllotEmployeesSpecialties();
        }

        /// <summary>
        /// 获取可分配任务的所有员工
        /// </summary>
        /// <returns></returns>
        public DataSet GetCanAssignEmployees(string orderBy)
        {
            return tacdDal.GetCanAssignEmployees(orderBy);
        }

        /// <summary>
        /// 获取所有未完成的任务
        /// </summary>
        /// <returns></returns>
        public DataSet GetNotFinishProjects()
        {
            return tacdDal.GetNotFinishProjects();
        }

        /// <summary>
        /// 获取时间充足的员工此前做过的任务
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public DataSet GetPreviousTask(string employees)
        {
            return tacdDal.GetPreviousTask(employees);
        }

        /// <summary>
        /// 获取所有的专业质量分
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllTaskAssignDetails()
        {
            return tacdDal.GetAllTaskAssignDetails();
        }

        /// <summary>
        /// 获取指定的几位员工的预期目标工资
        /// </summary>
        /// <param name="employeeIDs">员工ID集</param>
        /// <returns></returns>
        public DataSet GetAllCurrentAmount(string employeeIDs)
        {
            return tacdDal.GetAllCurrentAmount(employeeIDs);
        }
    }
}
