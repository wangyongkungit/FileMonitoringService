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
    public class RightDownDAL
    {
        public RightDownDAL()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from rightdown");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(RightDown model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into rightdown(");
            strSql.Append("ID,FROMVALUE,TOVALUE,RIGHTDOWNPERCENT)");
            strSql.Append(" values (");
            strSql.Append("@ID,@FROMVALUE,@TOVALUE,@RIGHTDOWNPERCENT)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40),
					new MySqlParameter("@FROMVALUE", MySqlDbType.Int32,3),
					new MySqlParameter("@TOVALUE", MySqlDbType.Int32,3),
					new MySqlParameter("@RIGHTDOWNPERCENT", MySqlDbType.Decimal,3)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.FROMVALUE;
            parameters[2].Value = model.TOVALUE;
            parameters[3].Value = model.RIGHTDOWNPERCENT;

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
        public bool Update(RightDown model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update rightdown set ");
            strSql.Append("FROMVALUE=@FROMVALUE,");
            strSql.Append("TOVALUE=@TOVALUE,");
            strSql.Append("RIGHTDOWNPERCENT=@RIGHTDOWNPERCENT");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@FROMVALUE", MySqlDbType.Int32,3),
					new MySqlParameter("@TOVALUE", MySqlDbType.Int32,3),
					new MySqlParameter("@RIGHTDOWNPERCENT", MySqlDbType.Decimal,3),
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)};
            parameters[0].Value = model.FROMVALUE;
            parameters[1].Value = model.TOVALUE;
            parameters[2].Value = model.RIGHTDOWNPERCENT;
            parameters[3].Value = model.ID;

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
            strSql.Append("delete from rightdown ");
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
            strSql.Append("delete from rightdown ");
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
        public RightDown GetModel(string ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,FROMVALUE,TOVALUE,RIGHTDOWNPERCENT from rightdown ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            RightDown model = new RightDown();
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
        public RightDown DataRowToModel(DataRow row)
        {
            RightDown model = new RightDown();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["FROMVALUE"] != null && row["FROMVALUE"].ToString() != "")
                {
                    model.FROMVALUE = int.Parse(row["FROMVALUE"].ToString());
                }
                if (row["TOVALUE"] != null && row["TOVALUE"].ToString() != "")
                {
                    model.TOVALUE = int.Parse(row["TOVALUE"].ToString());
                }
                if (row["RIGHTDOWNPERCENT"] != null && row["RIGHTDOWNPERCENT"].ToString() != "")
                {
                    model.RIGHTDOWNPERCENT = decimal.Parse(row["RIGHTDOWNPERCENT"].ToString());
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
            strSql.Append("select ID,FROMVALUE,TOVALUE,RIGHTDOWNPERCENT ");
            strSql.Append(" FROM rightdown ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.ID desc");
            }
            strSql.Append(")AS Row, T.*  from rightdown T ");
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

        #endregion  ExtensionMethod
    }
}
