using FileMonitoringService.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringDAL
{
    public class EmployeeProportionDAL
    {
        public EmployeeProportionDAL()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from employeeproportion");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)			};
			parameters[0].Value = ID;

			return DbHelperMySQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(EmployeeProportion model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into employeeproportion(");
			strSql.Append("ID,EMPLOYEEID,PROPORTION,PARENTEMPLOYEEID,ISBRANCHLEADER)");
			strSql.Append(" values (");
			strSql.Append("@ID,@EMPLOYEEID,@PROPORTION,@PARENTEMPLOYEEID,@ISBRANCHLEADER)");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36),
					new MySqlParameter("@EMPLOYEEID", MySqlDbType.VarChar,36),
					new MySqlParameter("@PROPORTION", MySqlDbType.Decimal,3),
					new MySqlParameter("@PARENTEMPLOYEEID", MySqlDbType.VarChar,36),
					new MySqlParameter("@ISBRANCHLEADER", MySqlDbType.Bit)};
			parameters[0].Value = model.ID;
			parameters[1].Value = model.EMPLOYEEID;
			parameters[2].Value = model.PROPORTION;
			parameters[3].Value = model.PARENTEMPLOYEEID;
			parameters[4].Value = model.ISBRANCHLEADER;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(EmployeeProportion model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update employeeproportion set ");
			strSql.Append("EMPLOYEEID=@EMPLOYEEID,");
			strSql.Append("PROPORTION=@PROPORTION,");
			strSql.Append("PARENTEMPLOYEEID=@PARENTEMPLOYEEID,");
			strSql.Append("ISBRANCHLEADER=@ISBRANCHLEADER");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@EMPLOYEEID", MySqlDbType.VarChar,36),
					new MySqlParameter("@PROPORTION", MySqlDbType.Decimal,3),
					new MySqlParameter("@PARENTEMPLOYEEID", MySqlDbType.VarChar,36),
					new MySqlParameter("@ISBRANCHLEADER", MySqlDbType.Bit),
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)};
			parameters[0].Value = model.EMPLOYEEID;
			parameters[1].Value = model.PROPORTION;
			parameters[2].Value = model.PARENTEMPLOYEEID;
			parameters[3].Value = model.ISBRANCHLEADER;
			parameters[4].Value = model.ID;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from employeeproportion ");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)			};
			parameters[0].Value = ID;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from employeeproportion ");
			strSql.Append(" where ID in ("+IDlist + ")  ");
			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString());
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public EmployeeProportion GetModel(string ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,EMPLOYEEID,PROPORTION,PARENTEMPLOYEEID,ISBRANCHLEADER from employeeproportion ");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)			};
			parameters[0].Value = ID;

			EmployeeProportion model=new EmployeeProportion();
			DataSet ds=DbHelperMySQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public EmployeeProportion DataRowToModel(DataRow row)
		{
			EmployeeProportion model=new EmployeeProportion();
			if (row != null)
			{
				if(row["ID"]!=null)
				{
					model.ID=row["ID"].ToString();
				}
				if(row["EMPLOYEEID"]!=null)
				{
					model.EMPLOYEEID=row["EMPLOYEEID"].ToString();
				}
				if(row["PROPORTION"]!=null && row["PROPORTION"].ToString()!="")
				{
					model.PROPORTION=decimal.Parse(row["PROPORTION"].ToString());
				}
				if(row["PARENTEMPLOYEEID"]!=null)
				{
					model.PARENTEMPLOYEEID=row["PARENTEMPLOYEEID"].ToString();
				}
				if(row["ISBRANCHLEADER"]!=null && row["ISBRANCHLEADER"].ToString()!="")
				{
					if((row["ISBRANCHLEADER"].ToString()=="1")||(row["ISBRANCHLEADER"].ToString().ToLower()=="true"))
					{
						model.ISBRANCHLEADER=true;
					}
					else
					{
						model.ISBRANCHLEADER=false;
					}
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,EMPLOYEEID,PROPORTION,PARENTEMPLOYEEID,ISBRANCHLEADER ");
			strSql.Append(" FROM employeeproportion ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperMySQL.Query(strSql.ToString());
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.ID desc");
			}
			strSql.Append(")AS Row, T.*  from employeeproportion T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperMySQL.Query(strSql.ToString());
		}

		#endregion  BasicMethod
		#region  ExtensionMethod
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetProportionByEmployeeID(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,EMPLOYEEID,PROPORTION,PARENTEMPLOYEEID,ISBRANCHLEADER ");
            strSql.Append(" FROM employeeproportion ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 根据员工ID计算出提成比例
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public decimal CalculateProportionByEmployeeID(string employeeID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT IFNULL(sum(proportion), 0.3) proportion FROM employeeproportion WHERE EMPLOYEEID = @employeeID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@employeeID", MySqlDbType.VarChar,36)			};
            parameters[0].Value = employeeID;
            object obj = DbHelperMySQL.GetSingle(strSql.ToString(), parameters);
            decimal proportion = Convert.ToDecimal(obj);
            return proportion;
        }
		#endregion  ExtensionMethod
    }
}
