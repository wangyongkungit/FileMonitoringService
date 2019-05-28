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
    /// <summary>
    /// 数据访问类:EmployeeAccount
    /// </summary>
    public partial class EmployeeAccountDAL
    {
        public EmployeeAccountDAL()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from employeeaccount");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)			};
            parameters[0].Value = ID;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(EmployeeAccount model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into employeeaccount(");
            strSql.Append("ID,AMOUNT,PAIDAMOUNT,SURPLUSAMOUNT,OTHERSAMOUNT,CREATEDATE,EMPLOYEEID,LASTUPDATEDATE,ISDELETED)");
            strSql.Append(" values (");
            strSql.Append("@ID,@AMOUNT,@PAIDAMOUNT,@SURPLUSAMOUNT,@OTHERSAMOUNT,@CREATEDATE,@EMPLOYEEID,@LASTUPDATEDATE,@ISDELETED)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36),
					new MySqlParameter("@AMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@PAIDAMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@SURPLUSAMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@OTHERSAMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@CREATEDATE", MySqlDbType.DateTime),
					new MySqlParameter("@EMPLOYEEID", MySqlDbType.VarChar,36),
					new MySqlParameter("@LASTUPDATEDATE", MySqlDbType.DateTime),
					new MySqlParameter("@ISDELETED", MySqlDbType.Bit)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.AMOUNT;
            parameters[2].Value = model.PAIDAMOUNT;
            parameters[3].Value = model.SURPLUSAMOUNT;
            parameters[4].Value = model.OTHERSAMOUNT;
            parameters[5].Value = model.CREATEDATE;
            parameters[6].Value = model.EMPLOYEEID;
            parameters[7].Value = model.LASTUPDATEDATE;
            parameters[8].Value = model.ISDELETED;

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
        public bool Update(EmployeeAccount model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update employeeaccount set ");
            strSql.Append("AMOUNT=@AMOUNT,");
            strSql.Append("PAIDAMOUNT=@PAIDAMOUNT,");
            strSql.Append("SURPLUSAMOUNT=@SURPLUSAMOUNT,");
            strSql.Append("OTHERSAMOUNT=@OTHERSAMOUNT,");
            strSql.Append("CREATEDATE=@CREATEDATE,");
            strSql.Append("EMPLOYEEID=@EMPLOYEEID,");
            strSql.Append("LASTUPDATEDATE=@LASTUPDATEDATE,");
            strSql.Append("ISDELETED=@ISDELETED");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@PAIDAMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@SURPLUSAMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@OTHERSAMOUNT", MySqlDbType.Decimal,12),
					new MySqlParameter("@CREATEDATE", MySqlDbType.DateTime),
					new MySqlParameter("@EMPLOYEEID", MySqlDbType.VarChar,36),
					new MySqlParameter("@LASTUPDATEDATE", MySqlDbType.DateTime),
					new MySqlParameter("@ISDELETED", MySqlDbType.Bit),
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)};
            parameters[0].Value = model.AMOUNT;
            parameters[1].Value = model.PAIDAMOUNT;
            parameters[2].Value = model.SURPLUSAMOUNT;
            parameters[3].Value = model.OTHERSAMOUNT;
            parameters[4].Value = model.CREATEDATE;
            parameters[5].Value = model.EMPLOYEEID;
            parameters[6].Value = model.LASTUPDATEDATE;
            parameters[7].Value = model.ISDELETED;
            parameters[8].Value = model.ID;

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
            strSql.Append("delete from employeeaccount ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)			};
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
            strSql.Append("delete from employeeaccount ");
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
        public EmployeeAccount GetModel(string ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,AMOUNT,PAIDAMOUNT,SURPLUSAMOUNT,OTHERSAMOUNT,CREATEDATE,EMPLOYEEID,LASTUPDATEDATE,ISDELETED from employeeaccount ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,36)			};
            parameters[0].Value = ID;

            EmployeeAccount model = new EmployeeAccount();
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
        public EmployeeAccount DataRowToModel(DataRow row)
        {
            EmployeeAccount model = new EmployeeAccount();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["AMOUNT"] != null && row["AMOUNT"].ToString() != "")
                {
                    model.AMOUNT = decimal.Parse(row["AMOUNT"].ToString());
                }
                if (row["PAIDAMOUNT"] != null && row["PAIDAMOUNT"].ToString() != "")
                {
                    model.PAIDAMOUNT = decimal.Parse(row["PAIDAMOUNT"].ToString());
                }
                if (row["SURPLUSAMOUNT"] != null && row["SURPLUSAMOUNT"].ToString() != "")
                {
                    model.SURPLUSAMOUNT = decimal.Parse(row["SURPLUSAMOUNT"].ToString());
                }
                if (row["OTHERSAMOUNT"] != null && row["OTHERSAMOUNT"].ToString() != "")
                {
                    model.OTHERSAMOUNT = decimal.Parse(row["OTHERSAMOUNT"].ToString());
                }
                if (row["CREATEDATE"] != null && row["CREATEDATE"].ToString() != "")
                {
                    model.CREATEDATE = DateTime.Parse(row["CREATEDATE"].ToString());
                }
                if (row["EMPLOYEEID"] != null)
                {
                    model.EMPLOYEEID = row["EMPLOYEEID"].ToString();
                }
                if (row["LASTUPDATEDATE"] != null && row["LASTUPDATEDATE"].ToString() != "")
                {
                    model.LASTUPDATEDATE = DateTime.Parse(row["LASTUPDATEDATE"].ToString());
                }
                if (row["ISDELETED"] != null && row["ISDELETED"].ToString() != "")
                {
                    if ((row["ISDELETED"].ToString() == "1") || (row["ISDELETED"].ToString().ToLower() == "true"))
                    {
                        model.ISDELETED = true;
                    }
                    else
                    {
                        model.ISDELETED = false;
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,AMOUNT,PAIDAMOUNT,SURPLUSAMOUNT,OTHERSAMOUNT,CREATEDATE,EMPLOYEEID,LASTUPDATEDATE,ISDELETED ");
            strSql.Append(" FROM employeeaccount ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM employeeaccount ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperMySQL.GetSingle(strSql.ToString());
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
            strSql.Append(")AS Row, T.*  from employeeaccount T ");
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