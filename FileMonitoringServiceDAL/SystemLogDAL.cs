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
    public class SystemLogDAL
    {
        public SystemLogDAL()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from systemlog");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(SystemLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into systemlog(");
            strSql.Append("ID,OPERATETYPE,OPERATECONTENT,CREATEDATE,EMPLOYEEID,IPADDRESS,PHYSICALADDRESS)");
            strSql.Append(" values (");
            strSql.Append("@ID,@OPERATETYPE,@OPERATECONTENT,@CREATEDATE,@EMPLOYEEID,@IPADDRESS,@PHYSICALADDRESS)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40),
					new MySqlParameter("@OPERATETYPE", MySqlDbType.VarChar,2),
					new MySqlParameter("@OPERATECONTENT", MySqlDbType.VarChar,2000),
					new MySqlParameter("@CREATEDATE", MySqlDbType.DateTime),
					new MySqlParameter("@EMPLOYEEID", MySqlDbType.VarChar,40),
					new MySqlParameter("@IPADDRESS", MySqlDbType.VarChar,40),
					new MySqlParameter("@PHYSICALADDRESS", MySqlDbType.VarChar,40)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.OPERATETYPE;
            parameters[2].Value = model.OPERATECONTENT;
            parameters[3].Value = model.CREATEDATE;
            parameters[4].Value = model.EMPLOYEEID;
            parameters[5].Value = model.IPADDRESS;
            parameters[6].Value = model.PHYSICALADDRESS;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
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
        public bool Update(SystemLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update systemlog set ");
            strSql.Append("OPERATETYPE=@OPERATETYPE,");
            strSql.Append("OPERATECONTENT=@OPERATECONTENT,");
            strSql.Append("CREATEDATE=@CREATEDATE,");
            strSql.Append("EMPLOYEEID=@EMPLOYEEID,");
            strSql.Append("IPADDRESS=@IPADDRESS,");
            strSql.Append("PHYSICALADDRESS=@PHYSICALADDRESS");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@OPERATETYPE", MySqlDbType.VarChar,2),
					new MySqlParameter("@OPERATECONTENT", MySqlDbType.VarChar,2000),
					new MySqlParameter("@CREATEDATE", MySqlDbType.DateTime),
					new MySqlParameter("@EMPLOYEEID", MySqlDbType.VarChar,40),
					new MySqlParameter("@IPADDRESS", MySqlDbType.VarChar,40),
					new MySqlParameter("@PHYSICALADDRESS", MySqlDbType.VarChar,40),
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)};
            parameters[0].Value = model.OPERATETYPE;
            parameters[1].Value = model.OPERATECONTENT;
            parameters[2].Value = model.CREATEDATE;
            parameters[3].Value = model.EMPLOYEEID;
            parameters[4].Value = model.IPADDRESS;
            parameters[5].Value = model.PHYSICALADDRESS;
            parameters[6].Value = model.ID;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
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

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from systemlog ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
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
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from systemlog ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString());
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
        public SystemLog GetModel(string ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,OPERATETYPE,OPERATECONTENT,CREATEDATE,EMPLOYEEID,IPADDRESS,PHYSICALADDRESS from systemlog ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            SystemLog model = new SystemLog();
            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
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
        public SystemLog DataRowToModel(DataRow row)
        {
            SystemLog model = new SystemLog();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["OPERATETYPE"] != null)
                {
                    model.OPERATETYPE = row["OPERATETYPE"].ToString();
                }
                if (row["OPERATECONTENT"] != null)
                {
                    model.OPERATECONTENT = row["OPERATECONTENT"].ToString();
                }
                if (row["CREATEDATE"] != null && row["CREATEDATE"].ToString() != "")
                {
                    model.CREATEDATE = DateTime.Parse(row["CREATEDATE"].ToString());
                }
                if (row["EMPLOYEEID"] != null)
                {
                    model.EMPLOYEEID = row["EMPLOYEEID"].ToString();
                }
                if (row["IPADDRESS"] != null)
                {
                    model.IPADDRESS = row["IPADDRESS"].ToString();
                }
                if (row["PHYSICALADDRESS"] != null)
                {
                    model.PHYSICALADDRESS = row["PHYSICALADDRESS"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,OPERATETYPE,OPERATECONTENT,CREATEDATE,EMPLOYEEID,IPADDRESS,PHYSICALADDRESS ");
            strSql.Append(" FROM systemlog ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /*/// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql=new StringBuilder();
            strSql.Append("select count(1) FROM systemlog ");
            if(strWhere.Trim()!="")
            {
                strSql.Append(" where "+strWhere);
            }
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
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
            strSql.Append(")AS Row, T.*  from systemlog T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperMySQL.Query(strSql.ToString());
        }*/

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
            parameters[0].Value = "systemlog";
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
