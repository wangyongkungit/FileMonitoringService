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
    public class WeightsConfigDAL
    {
        public WeightsConfigDAL()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from weightsconfig");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(WeightsConfig model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into weightsconfig(");
            strSql.Append("ID,ITEMKEY,ITEMNAME,ITEMVALUE)");
            strSql.Append(" values (");
            strSql.Append("@ID,@ITEMKEY,@ITEMNAME,@ITEMVALUE)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40),
					new MySqlParameter("@ITEMKEY", MySqlDbType.VarChar,2),
					new MySqlParameter("@ITEMNAME", MySqlDbType.VarChar,24),
					new MySqlParameter("@ITEMVALUE", MySqlDbType.Decimal,5)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.ITEMKEY;
            parameters[2].Value = model.ITEMNAME;
            parameters[3].Value = model.ITEMVALUE;

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
        public bool Update(WeightsConfig model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update weightsconfig set ");
            strSql.Append("ITEMKEY=@ITEMKEY,");
            strSql.Append("ITEMNAME=@ITEMNAME,");
            strSql.Append("ITEMVALUE=@ITEMVALUE");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ITEMKEY", MySqlDbType.VarChar,2),
					new MySqlParameter("@ITEMNAME", MySqlDbType.VarChar,24),
					new MySqlParameter("@ITEMVALUE", MySqlDbType.Decimal,5),
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)};
            parameters[0].Value = model.ITEMKEY;
            parameters[1].Value = model.ITEMNAME;
            parameters[2].Value = model.ITEMVALUE;
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
            strSql.Append("delete from weightsconfig ");
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
            strSql.Append("delete from weightsconfig ");
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
        public WeightsConfig GetModel(string ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,ITEMKEY,ITEMNAME,ITEMVALUE from weightsconfig ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            WeightsConfig model = new WeightsConfig();
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
        public WeightsConfig DataRowToModel(DataRow row)
        {
            WeightsConfig model = new WeightsConfig();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["ITEMKEY"] != null)
                {
                    model.ITEMKEY = row["ITEMKEY"].ToString();
                }
                if (row["ITEMNAME"] != null)
                {
                    model.ITEMNAME = row["ITEMNAME"].ToString();
                }
                if (row["ITEMVALUE"] != null && row["ITEMVALUE"].ToString() != "")
                {
                    model.ITEMVALUE = decimal.Parse(row["ITEMVALUE"].ToString());
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
            strSql.Append("select ID,ITEMKEY,ITEMNAME,ITEMVALUE ");
            strSql.Append(" FROM weightsconfig ");
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
            strSql.Append(")AS Row, T.*  from weightsconfig T ");
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
