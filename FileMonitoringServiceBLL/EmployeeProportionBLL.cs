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
    public class EmployeeProportionBLL
    {
        private readonly EmployeeProportionDAL dal=new EmployeeProportionDAL();
        public EmployeeProportionBLL()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			return dal.Exists(ID);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(EmployeeProportion model)
		{
			return dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(EmployeeProportion model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
			
			return dal.Delete(ID);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			return dal.DeleteList(IDlist );
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public EmployeeProportion GetModel(string ID)
		{
			
			return dal.GetModel(ID);
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<EmployeeProportion> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<EmployeeProportion> DataTableToList(DataTable dt)
		{
			List<EmployeeProportion> modelList = new List<EmployeeProportion>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				EmployeeProportion model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = dal.DataRowToModel(dt.Rows[n]);
					if (model != null)
					{
						modelList.Add(model);
					}
				}
			}
			return modelList;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			return dal.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);
		}

		#endregion  BasicMethod
		#region  ExtensionMethod
        /// <summary>
        /// 根据员工编号获得数据列表
        /// </summary>
        public List<EmployeeProportion> GetModelListByEmployeeNo(string employeeNo)
        {
            string strWhere = " EMPLOYEEID IN ( SELECT ID FROM employee WHERE EMPLOYEENO = '" + employeeNo + "')";
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }

        /// <summary>
        /// 根据员工ID计算出提成比例
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public decimal CalculateProportionByEmployeeID(string employeeID)
        {
            return dal.CalculateProportionByEmployeeID(employeeID);
        }
		#endregion  ExtensionMethod
    }
}
