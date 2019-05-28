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
    public class TaskAssignFailLogDAL
    {
        public TaskAssignFailLogDAL()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from taskassignfaillog");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
			parameters[0].Value = ID;

			return DbHelperMySQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(TaskAssignFailLog model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into taskassignfaillog(");
			strSql.Append("ID,PROJECTID,FAILCOUNT,ISREMIND)");
			strSql.Append(" values (");
			strSql.Append("@ID,@PROJECTID,@FAILCOUNT,@ISREMIND)");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40),
					new MySqlParameter("@PROJECTID", MySqlDbType.VarChar,40),
					new MySqlParameter("@FAILCOUNT", MySqlDbType.Int32,2),
					new MySqlParameter("@ISREMIND", MySqlDbType.Int32,1)};
			parameters[0].Value = model.ID;
			parameters[1].Value = model.PROJECTID;
			parameters[2].Value = model.FAILCOUNT;
			parameters[3].Value = model.ISREMIND;

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
		public bool Update(TaskAssignFailLog model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update taskassignfaillog set ");
			strSql.Append("PROJECTID=@PROJECTID,");
			strSql.Append("FAILCOUNT=@FAILCOUNT,");
			strSql.Append("ISREMIND=@ISREMIND");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@PROJECTID", MySqlDbType.VarChar,40),
					new MySqlParameter("@FAILCOUNT", MySqlDbType.Int32,2),
					new MySqlParameter("@ISREMIND", MySqlDbType.Int32,1),
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)};
			parameters[0].Value = model.PROJECTID;
			parameters[1].Value = model.FAILCOUNT;
			parameters[2].Value = model.ISREMIND;
			parameters[3].Value = model.ID;

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
			strSql.Append("delete from taskassignfaillog ");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
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
			strSql.Append("delete from taskassignfaillog ");
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
		public TaskAssignFailLog GetModel(string ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,PROJECTID,FAILCOUNT,ISREMIND from taskassignfaillog ");
			strSql.Append(" where ID=@ID ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
			parameters[0].Value = ID;

			TaskAssignFailLog model=new TaskAssignFailLog();
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
		public TaskAssignFailLog DataRowToModel(DataRow row)
		{
			TaskAssignFailLog model=new TaskAssignFailLog();
			if (row != null)
			{
				if(row["ID"]!=null)
				{
					model.ID=row["ID"].ToString();
				}
				if(row["PROJECTID"]!=null)
				{
					model.PROJECTID=row["PROJECTID"].ToString();
				}
				if(row["FAILCOUNT"]!=null && row["FAILCOUNT"].ToString()!="")
				{
					model.FAILCOUNT=int.Parse(row["FAILCOUNT"].ToString());
				}
				if(row["ISREMIND"]!=null && row["ISREMIND"].ToString()!="")
				{
					model.ISREMIND=int.Parse(row["ISREMIND"].ToString());
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
			strSql.Append("select ID,PROJECTID,FAILCOUNT,ISREMIND ");
			strSql.Append(" FROM taskassignfaillog ");
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
			strSql.Append(")AS Row, T.*  from taskassignfaillog T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperMySQL.Query(strSql.ToString());
		}

		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			MySqlParameter[] parameters = {
					new MySqlParameter("@tblName", MySqlDbType.VarChar, 255),
					new MySqlParameter("@fldName", MySqlDbType.VarChar, 255),
					new MySqlParameter("@PageSize", MySqlDbType.Int32),
					new MySqlParameter("@PageIndex", MySqlDbType.Int32),
					new MySqlParameter("@IsReCount", MySqlDbType.Bit),
					new MySqlParameter("@OrderType", MySqlDbType.Bit),
					new MySqlParameter("@strWhere", MySqlDbType.VarChar,1000),
					};
			parameters[0].Value = "taskassignfaillog";
			parameters[1].Value = "ID";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperMySQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
    }
}
