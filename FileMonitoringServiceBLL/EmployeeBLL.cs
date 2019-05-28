using FileMonitoringDAL;
using FileMonitoringService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringServiceBLL
{
    public class EmployeeBLL
    {
        EmployeeDAL eDal = new EmployeeDAL();

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public DataTable GetUser(string userName, string passWord)
        {
            return eDal.GetUser(userName, passWord);
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public bool UpdatePassword(string ID, string passWord)
        {
            return eDal.UpdatePassword(ID, passWord);
        }

        /// <summary>
        /// 判断员工编号是否存在
        /// </summary>
        /// <param name="employeeNO">员工编号</param>
        /// <returns></returns>
        public bool IsExist(string employeeNO)
        {
            return eDal.IsExist(employeeNO);
        }

        /// <summary>
        /// 获取使用到的最大编号
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public string GetMaxEmployeeNO(string userType)
        {
            return eDal.GetMaxEmployeeNO(userType);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Employee model)
        {
            return eDal.Add(model);
        }

        /// <summary>
        /// 获得一个对象实体
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Employee GetModel(string ID)
        {
            return eDal.GetModel(ID);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="strSort"></param>
        /// <returns></returns>
        public DataSet GetList(string strWhere, string strSort)
        {
            return eDal.GetList(strWhere, strSort);
        }

        /// <summary>
        /// 获得数据列表，组合员工编号和姓名
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="strSort"></param>
        /// <returns></returns>
        public DataSet GetListUnionNoAndName(string strWhere, string strSort)
        {
            return eDal.GetListUnionNoAndName(strWhere, strSort);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(Employee model)
        {
            return eDal.Update(model);
        }
    }
}
