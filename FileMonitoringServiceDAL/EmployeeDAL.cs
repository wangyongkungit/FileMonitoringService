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
    public class EmployeeDAL
    {
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public DataTable GetUser(string userName, string passWord)
        {
            string strSql = string.Format(@"SELECT emp.ID,emp.EMPLOYEENO,emp.`NAME`,
                                 r.ID roleID,r.ROLENAME,
                                 m.MENUNAME,m.MENUPATH
                                 from employee emp
                                 INNER JOIN employeerole emprole
                                 on emp.id=emprole.employeeid
                                 INNER JOIN role r
                                 on emprole.roleid=r.id
                                 INNER JOIN rolemenumap rmm
                                 ON r.ID=rmm.ROLEID
                                 INNER JOIN menu m
                                 ON rmm.MENUID=m.ID
	                             where EMPLOYEENO='{0}' and `PASSWORD`='{1}'", userName, passWord);
            DataSet ds = MySqlHelper.GetDataSet(strSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public bool UpdatePassword(string ID, string passWord)
        {
            string strSql = string.Format(@"update employee set password='{0}' where ID='{1}'", passWord, ID);
            int r = DbHelperMySQL.ExecuteSql(strSql);
            return r > 0;
        }

        /// <summary>
        /// 判断员工编号是否存在
        /// </summary>
        /// <param name="employeeNO">员工编号</param>
        /// <returns></returns>
        public bool IsExist(string employeeNO)
        {
            string sql = string.Format("SELECT COUNT(*) FROM EMPLOYEE WHERE EMPLOYEENO = '{0}'", employeeNO);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            int amount = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                amount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return amount > 0;
        }

        /// <summary>
        /// 获取最大编号值
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public string GetMaxEmployeeNO(string userType)
        {
            string strSql = string.Format(@"SELECT MAX(RIGHT(employeeno,3)) FROM employee WHERE available = 1 AND type='{0}'", userType);
            string maxNo = string.Empty;
            try
            {
                DataSet ds = MySqlHelper.GetDataSet(strSql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    maxNo = ds.Tables[0].Rows[0][0].ToString();
                }
                return maxNo;
            }
            catch (Exception)
            {
                return maxNo;
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Employee model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into employee(");
            strSql.Append("ID,EMPLOYEENO,PASSWORD,NAME,SEX,BIRTHDATE,NATIVEPLACE,MOBILEPHONE,ADDRESS,EMAIL,BANKCARD,DEPARTMENTID,POLITICALSTATUS,TYPE,AVAILABLE,DINGTALKUSERID)");
            strSql.Append(" values (");
            strSql.Append("@ID,@EMPLOYEENO,@PASSWORD,@NAME,@SEX,@BIRTHDATE,@NATIVEPLACE,@MOBILEPHONE,@ADDRESS,@EMAIL,@BANKCARD,@DEPARTMENTID,@POLITICALSTATUS,@TYPE,@AVAILABLE,@DINGTALKUSERID)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40),
					new MySqlParameter("@EMPLOYEENO", MySqlDbType.VarChar,50),
					new MySqlParameter("@PASSWORD", MySqlDbType.VarChar,200),
					new MySqlParameter("@NAME", MySqlDbType.VarChar,100),
					new MySqlParameter("@SEX", MySqlDbType.Bit),
					new MySqlParameter("@BIRTHDATE", MySqlDbType.Date),
					new MySqlParameter("@NATIVEPLACE", MySqlDbType.VarChar,40),
					new MySqlParameter("@MOBILEPHONE", MySqlDbType.VarChar,20),
					new MySqlParameter("@ADDRESS", MySqlDbType.VarChar,255),
					new MySqlParameter("@EMAIL", MySqlDbType.VarChar,50),
					new MySqlParameter("@BANKCARD", MySqlDbType.VarChar,20),
					new MySqlParameter("@DEPARTMENTID", MySqlDbType.VarChar,40),
					new MySqlParameter("@POLITICALSTATUS", MySqlDbType.VarChar,20),
					new MySqlParameter("@TYPE", MySqlDbType.Decimal,1),
					new MySqlParameter("@AVAILABLE", MySqlDbType.Decimal,1),
					new MySqlParameter("@DINGTALKUSERID", MySqlDbType.VarChar,40)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.EMPLOYEENO;
            parameters[2].Value = model.PASSWORD;
            parameters[3].Value = model.NAME;
            parameters[4].Value = model.SEX;
            parameters[5].Value = model.BIRTHDATE;
            parameters[6].Value = model.NATIVEPLACE;
            parameters[7].Value = model.MOBILEPHONE;
            parameters[8].Value = model.ADDRESS;
            parameters[9].Value = model.EMAIL;
            parameters[10].Value = model.BANKCARD;
            parameters[11].Value = model.DEPARTMENTID;
            parameters[12].Value = model.POLITICALSTATUS;
            parameters[13].Value = model.TYPE;
            parameters[14].Value = model.AVAILABLE;
            parameters[15].Value = model.DINGTALKUSERID;

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
        /// 得到一个对象实体
        /// </summary>
        public Employee GetModel(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,EMPLOYEENO,PASSWORD,NAME,SEX,BIRTHDATE,NATIVEPLACE,MOBILEPHONE,ADDRESS,EMAIL,BANKCARD,DEPARTMENTID,POLITICALSTATUS,TYPE,AVAILABLE,DINGTALKUSERID from employee ");
            strSql.Append(" where AVAILABLE = 1 AND ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)			};
            parameters[0].Value = ID;

            Employee model = new Employee();
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
        public Employee DataRowToModel(DataRow row)
        {
            Employee model = new Employee();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["EMPLOYEENO"] != null)
                {
                    model.EMPLOYEENO = row["EMPLOYEENO"].ToString();
                }
                if (row["PASSWORD"] != null)
                {
                    model.PASSWORD = row["PASSWORD"].ToString();
                }
                if (row["NAME"] != null)
                {
                    model.NAME = row["NAME"].ToString();
                }
                if (row["SEX"] != null && row["SEX"].ToString() != "")
                {
                    if ((row["SEX"].ToString() == "1") || (row["SEX"].ToString().ToLower() == "true"))
                    {
                        model.SEX = true;
                    }
                    else
                    {
                        model.SEX = false;
                    }
                }
                if (row["BIRTHDATE"] != null && row["BIRTHDATE"].ToString() != "")
                {
                    model.BIRTHDATE = DateTime.Parse(row["BIRTHDATE"].ToString());
                }
                if (row["NATIVEPLACE"] != null)
                {
                    model.NATIVEPLACE = row["NATIVEPLACE"].ToString();
                }
                if (row["MOBILEPHONE"] != null)
                {
                    model.MOBILEPHONE = row["MOBILEPHONE"].ToString();
                }
                if (row["ADDRESS"] != null)
                {
                    model.ADDRESS = row["ADDRESS"].ToString();
                }
                if (row["EMAIL"] != null)
                {
                    model.EMAIL = row["EMAIL"].ToString();
                }
                if (row["BANKCARD"] != null)
                {
                    model.BANKCARD = row["BANKCARD"].ToString();
                }
                if (row["DEPARTMENTID"] != null)
                {
                    model.DEPARTMENTID = row["DEPARTMENTID"].ToString();
                }
                if (row["POLITICALSTATUS"] != null)
                {
                    model.POLITICALSTATUS = row["POLITICALSTATUS"].ToString();
                }
                if (row["TYPE"] != null && row["TYPE"].ToString() != "")
                {
                    model.TYPE = decimal.Parse(row["TYPE"].ToString());
                }
                if (row["AVAILABLE"] != null && row["AVAILABLE"].ToString() != "")
                {
                    model.AVAILABLE = decimal.Parse(row["AVAILABLE"].ToString());
                }
                if (row["DINGTALKUSERID"] != null)
                {
                    model.DINGTALKUSERID = row["DINGTALKUSERID"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="strSort"></param>
        /// <returns></returns>
        public DataSet GetList(string strWhere, string strSort)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,EMPLOYEENO,PASSWORD,NAME,SEX,BIRTHDATE,NATIVEPLACE,MOBILEPHONE,ADDRESS,EMAIL,BANKCARD,DEPARTMENTID,POLITICALSTATUS,TYPE,AVAILABLE,DINGTALKUSERID ");
            strSql.Append(" FROM employee ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where AIVILABLE = 1 ");
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
            if (!string.IsNullOrEmpty(strSort.Trim()))
            {
                strSql.Append(" ORDER BY " + strSort);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得数据列表，组合员工编号和姓名
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="strSort"></param>
        /// <returns></returns>
        public DataSet GetListUnionNoAndName(string strWhere, string strSort)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,CONCAT_WS(' - ',EMPLOYEENO,NAME) NOANDNAME ");
            strSql.Append(" FROM employee ");
            strSql.Append(" where AVAILABLE = 1 and ID NOT IN (SELECT employeeId FROM taskassignconfig)");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(strWhere);
            }
            if (!string.IsNullOrEmpty(strSort.Trim()))
            {
                strSql.Append(" ORDER BY " + strSort);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Employee model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update employee set ");
            strSql.Append("EMPLOYEENO=@EMPLOYEENO,");
            strSql.Append("PASSWORD=@PASSWORD,");
            strSql.Append("NAME=@NAME,");
            strSql.Append("SEX=@SEX,");
            strSql.Append("BIRTHDATE=@BIRTHDATE,");
            strSql.Append("NATIVEPLACE=@NATIVEPLACE,");
            strSql.Append("MOBILEPHONE=@MOBILEPHONE,");
            strSql.Append("ADDRESS=@ADDRESS,");
            strSql.Append("EMAIL=@EMAIL,");
            strSql.Append("BANKCARD=@BANKCARD,");
            strSql.Append("DEPARTMENTID=@DEPARTMENTID,");
            strSql.Append("POLITICALSTATUS=@POLITICALSTATUS,");
            strSql.Append("TYPE=@TYPE,");
            strSql.Append("AVAILABLE=@AVAILABLE,");
            strSql.Append("DINGTALKUSERID=@DINGTALKUSERID");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EMPLOYEENO", MySqlDbType.VarChar,50),
					new MySqlParameter("@PASSWORD", MySqlDbType.VarChar,200),
					new MySqlParameter("@NAME", MySqlDbType.VarChar,100),
					new MySqlParameter("@SEX", MySqlDbType.Bit),
					new MySqlParameter("@BIRTHDATE", MySqlDbType.Date),
					new MySqlParameter("@NATIVEPLACE", MySqlDbType.VarChar,40),
					new MySqlParameter("@MOBILEPHONE", MySqlDbType.VarChar,20),
					new MySqlParameter("@ADDRESS", MySqlDbType.VarChar,255),
					new MySqlParameter("@EMAIL", MySqlDbType.VarChar,50),
					new MySqlParameter("@BANKCARD", MySqlDbType.VarChar,20),
					new MySqlParameter("@DEPARTMENTID", MySqlDbType.VarChar,40),
					new MySqlParameter("@POLITICALSTATUS", MySqlDbType.VarChar,20),
					new MySqlParameter("@TYPE", MySqlDbType.Decimal,1),
					new MySqlParameter("@AVAILABLE", MySqlDbType.Decimal,1),
					new MySqlParameter("@DINGTALKUSERID", MySqlDbType.VarChar,40),
					new MySqlParameter("@ID", MySqlDbType.VarChar,40)};
            parameters[0].Value = model.EMPLOYEENO;
            parameters[1].Value = model.PASSWORD;
            parameters[2].Value = model.NAME;
            parameters[3].Value = model.SEX;
            parameters[4].Value = model.BIRTHDATE;
            parameters[5].Value = model.NATIVEPLACE;
            parameters[6].Value = model.MOBILEPHONE;
            parameters[7].Value = model.ADDRESS;
            parameters[8].Value = model.EMAIL;
            parameters[9].Value = model.BANKCARD;
            parameters[10].Value = model.DEPARTMENTID;
            parameters[11].Value = model.POLITICALSTATUS;
            parameters[12].Value = model.TYPE;
            parameters[13].Value = model.AVAILABLE;
            parameters[14].Value = model.DINGTALKUSERID;
            parameters[15].Value = model.ID;

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
    }
}
