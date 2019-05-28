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
    public class FileCategoryDAL
    {
        public FileCategoryDAL()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from filecategory");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,36)           };
            parameters[0].Value = ID;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FileCategory model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into filecategory(");
            strSql.Append("ID,PROJECTID,CATEGORY,TITLE,DESCRIPTION,FOLDERNAME,CREATEDATE,CREATEUSER,EXPIREDATE,PARENTID,CLASSSORT,DIVISIONSORT,ORDERSORT)");
            strSql.Append(" values (");
            strSql.Append("@ID,@PROJECTID,@CATEGORY,@TITLE,@DESCRIPTION,@FOLDERNAME,@CREATEDATE,@CREATEUSER,@EXPIREDATE,@PARENTID,@CLASSSORT,@DIVISIONSORT,@ORDERSORT)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,36),
                    new MySqlParameter("@PROJECTID", MySqlDbType.VarChar,36),
                    new MySqlParameter("@CATEGORY", MySqlDbType.VarChar,3),
                    new MySqlParameter("@TITLE", MySqlDbType.VarChar,100),
                    new MySqlParameter("@DESCRIPTION", MySqlDbType.VarChar,255),
                    new MySqlParameter("@FOLDERNAME", MySqlDbType.VarChar,255),
                    new MySqlParameter("@CREATEDATE", MySqlDbType.DateTime),
                    new MySqlParameter("@CREATEUSER", MySqlDbType.VarChar,36),
                    new MySqlParameter("@EXPIREDATE", MySqlDbType.DateTime),
                    new MySqlParameter("@PARENTID", MySqlDbType.VarChar,36),
                    new MySqlParameter("@CLASSSORT", MySqlDbType.Int32,2),
                    new MySqlParameter("@DIVISIONSORT", MySqlDbType.Int32,2),
                    new MySqlParameter("@ORDERSORT", MySqlDbType.Int32,2)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.PROJECTID;
            parameters[2].Value = model.CATEGORY;
            parameters[3].Value = model.TITLE;
            parameters[4].Value = model.DESCRIPTION;
            parameters[5].Value = model.FOLDERNAME;
            parameters[6].Value = model.CREATEDATE;
            parameters[7].Value = model.CREATEUSER;
            parameters[8].Value = model.EXPIREDATE;
            parameters[9].Value = model.PARENTID;
            parameters[10].Value = model.CLASSSORT;
            parameters[11].Value = model.DIVISIONSORT;
            parameters[12].Value = model.ORDERSORT;

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
        public bool Update(FileCategory model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update filecategory set ");
            strSql.Append("PROJECTID=@PROJECTID,");
            strSql.Append("CATEGORY=@CATEGORY,");
            strSql.Append("TITLE=@TITLE,");
            strSql.Append("DESCRIPTION=@DESCRIPTION,");
            strSql.Append("FOLDERNAME=@FOLDERNAME,");
            strSql.Append("CREATEDATE=@CREATEDATE,");
            strSql.Append("CREATEUSER=@CREATEUSER,");
            strSql.Append("EXPIREDATE=@EXPIREDATE,");
            strSql.Append("PARENTID=@PARENTID,");
            strSql.Append("CLASSSORT=@CLASSSORT,");
            strSql.Append("DIVISIONSORT=@DIVISIONSORT,");
            strSql.Append("ORDERSORT=@ORDERSORT");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@PROJECTID", MySqlDbType.VarChar,36),
                    new MySqlParameter("@CATEGORY", MySqlDbType.VarChar,3),
                    new MySqlParameter("@TITLE", MySqlDbType.VarChar,100),
                    new MySqlParameter("@DESCRIPTION", MySqlDbType.VarChar,255),
                    new MySqlParameter("@FOLDERNAME", MySqlDbType.VarChar,255),
                    new MySqlParameter("@CREATEDATE", MySqlDbType.DateTime),
                    new MySqlParameter("@CREATEUSER", MySqlDbType.VarChar,36),
                    new MySqlParameter("@EXPIREDATE", MySqlDbType.DateTime),
                    new MySqlParameter("@PARENTID", MySqlDbType.VarChar,36),
                    new MySqlParameter("@CLASSSORT", MySqlDbType.Int32,2),
                    new MySqlParameter("@DIVISIONSORT", MySqlDbType.Int32,2),
                    new MySqlParameter("@ORDERSORT", MySqlDbType.Int32,2),
                    new MySqlParameter("@ID", MySqlDbType.VarChar,36)};
            parameters[0].Value = model.PROJECTID;
            parameters[1].Value = model.CATEGORY;
            parameters[2].Value = model.TITLE;
            parameters[3].Value = model.DESCRIPTION;
            parameters[4].Value = model.FOLDERNAME;
            parameters[5].Value = model.CREATEDATE;
            parameters[6].Value = model.CREATEUSER;
            parameters[7].Value = model.EXPIREDATE;
            parameters[8].Value = model.PARENTID;
            parameters[9].Value = model.CLASSSORT;
            parameters[10].Value = model.DIVISIONSORT;
            parameters[11].Value = model.ORDERSORT;
            parameters[12].Value = model.ID;

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
            strSql.Append("delete from filecategory ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,36)           };
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
            strSql.Append("delete from filecategory ");
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
        public FileCategory GetModel(string ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,PROJECTID,CATEGORY,TITLE,DESCRIPTION,FOLDERNAME,CREATEDATE,CREATEUSER,exPIREDATE,PARENTID,CLASSSORT,DIVISIONSORT,ORDERSORT from filecategory ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,36)           };
            parameters[0].Value = ID;

            FileCategory model = new FileCategory();
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
        public FileCategory DataRowToModel(DataRow row)
        {
            FileCategory model = new FileCategory();
            if (row != null)
            {
                if (row["ID"] != null)
                {
                    model.ID = row["ID"].ToString();
                }
                if (row["PROJECTID"] != null)
                {
                    model.PROJECTID = row["PROJECTID"].ToString();
                }
                if (row["CATEGORY"] != null)
                {
                    model.CATEGORY = row["CATEGORY"].ToString();
                }
                if (row["TITLE"] != null)
                {
                    model.TITLE = row["TITLE"].ToString();
                }
                if (row["DESCRIPTION"] != null)
                {
                    model.DESCRIPTION = row["DESCRIPTION"].ToString();
                }
                if (row["FOLDERNAME"] != null)
                {
                    model.FOLDERNAME = row["FOLDERNAME"].ToString();
                }
                if (row["CREATEDATE"] != null && row["CREATEDATE"].ToString() != "")
                {
                    model.CREATEDATE = DateTime.Parse(row["CREATEDATE"].ToString());
                }
                if (row["CREATEUSER"] != null)
                {
                    model.CREATEUSER = row["CREATEUSER"].ToString();
                }
                if (row["exPIREDATE"] != null && row["exPIREDATE"].ToString() != "")
                {
                    model.EXPIREDATE = DateTime.Parse(row["exPIREDATE"].ToString());
                }
                if (row["PARENTID"] != null)
                {
                    model.PARENTID = row["PARENTID"].ToString();
                }
                if (row["CLASSSORT"] != null && row["CLASSSORT"].ToString() != "")
                {
                    model.CLASSSORT = int.Parse(row["CLASSSORT"].ToString());
                }
                if (row["DIVISIONSORT"] != null && row["DIVISIONSORT"].ToString() != "")
                {
                    model.DIVISIONSORT = int.Parse(row["DIVISIONSORT"].ToString());
                }
                if (row["ORDERSORT"] != null && row["ORDERSORT"].ToString() != "")
                {
                    model.ORDERSORT = int.Parse(row["ORDERSORT"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere, string orderBy)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,PROJECTID,CATEGORY,TITLE,DESCRIPTION,FOLDERNAME,CREATEDATE,CREATEUSER,EXPIREDATE,PARENTID,CLASSSORT,DIVISIONSORT,ORDERSORT ");
            strSql.Append(" FROM filecategory ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                strSql.Append(orderBy);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM filecategory ");
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
            strSql.Append(")AS Row, T.*  from filecategory T ");
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
        /// 根据 fileHistoryId 获取 projectId
        /// </summary>
        public DataSet GetProjectIdByFileHistoryId(string fileHistoryId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT PROJECTID, Category FROM filecategory 
                            WHERE ID IN
                            (
                             SELECT PARENTID FROM filehistory WHERE ID = '" + fileHistoryId + "'  )");
            DataSet ds = DbHelperMySQL.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 根据proejctId和category获取orderSort
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public int GetOrderSort(string projectId, string category)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT IFNULL(max(ORDERSORT), 0) + 1 as ORDERSORT FROM filecategory WHERE projectId = @projectId AND CATEGORY = @category ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@projectId", MySqlDbType.String, 3),
                    new MySqlParameter("@category", MySqlDbType.VarChar, 3) };
            parameters[0].Value = projectId;
            parameters[1].Value = category;
            object obj = DbHelperMySQL.GetSingle(strSql.ToString(), parameters);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 根据 parentId 获取序号
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public int GetOrderSortForChildTab(string parentId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT ORDERSORT FROM filecategory WHERE ID = @parentId");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@parentId", MySqlDbType.VarChar,36) };
            parameters[0].Value = parentId;
            object obj = DbHelperMySQL.GetSingle(strSql.ToString(), parameters);
            return Convert.ToInt32(obj);
        }

//        /// <summary>
//        /// 获取回复tab列表
//        /// </summary>
//        /// <param name="categoryId">配置表中的categoryId</param>
//        /// <param name="parentId">针对哪个记录进行的回复</param>
//        /// <returns></returns>
//        public DataSet GetReplayToList(string projectId, string category)
//        {
//            // 需要对 parentCategory 进行转换，
//            string parentCategory = GlobalConfig.dicMapForSubTab[category].ToString();
//            string strSql = @"
//                            SELECT f.Id, f.CATEGORY CATEGORYId, f.TITLE, f.expireDate FROM filecategory f
//                            WHERE PROJECTID = @projectId AND CATEGORY = @parentCategory  
//                            AND f.ID NOT IN (  
//	                          SELECT PARENTID FROM filecategory
//                              WHERE PROJECTID = @projectId AND CATEGORY = @category and PARENTID <> '0' )  ORDER BY f.orderSort ";
//            MySqlParameter[] parameters = {
//                new MySqlParameter("@projectId", MySqlDbType.VarChar,36),
//                new MySqlParameter("@category",MySqlDbType.String, 3),
//                new MySqlParameter("@parentCategory", MySqlDbType.String,3)
//            };
//            parameters[0].Value = projectId;
//            parameters[1].Value = category;
//            parameters[2].Value = parentCategory;
//            DataSet ds = DbHelperMySQL.Query(strSql, parameters);
//            return ds;
//        }

        public DataSet GetExpireDateByProjectId(string projectId)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT fc.FOLDERNAME, fc.expireDate FROM filecategory fc
                                 WHERE projectid = '" + projectId + @"'
                                 AND parentId NOT IN(
                                    SELECT ID FROM filecategory WHERE projectId = '" + projectId + @"'
                                )
                                 ORDER BY CREATEDATE DESC LIMIT 0, 1");
            DataSet ds = DbHelperMySQL.Query(sbSql.ToString());
            return ds;
        }
        #endregion  ExtensionMethod
    }
}
