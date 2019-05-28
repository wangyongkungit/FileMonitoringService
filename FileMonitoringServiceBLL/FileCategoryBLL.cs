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
    public class FileCategoryBLL
    {
        private readonly FileCategoryDAL dal = new FileCategoryDAL();
        public FileCategoryBLL()
        { }
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
        public bool Add(FileCategory model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FileCategory model)
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
        public bool DeleteList(string IDlist)
        {
            return dal.DeleteList(IDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FileCategory GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere, string orderBy)
        {
            return dal.GetList(strWhere, orderBy);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FileCategory> GetModelList(string strWhere, string orderBy)
        {
            DataSet ds = dal.GetList(strWhere, orderBy);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FileCategory> DataTableToList(DataTable dt)
        {
            List<FileCategory> modelList = new List<FileCategory>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                FileCategory model;
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
            return GetList(string.Empty, string.Empty);
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            return dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            return dal.GetListByPage(strWhere, orderby, startIndex, endIndex);
        }

        #endregion  BasicMethod
        #region  ExtensionMethod
        /// <summary>
        /// 根据 fileHistoryId 获取 projectId
        /// </summary>
        public DataSet GetProjectIdByFileHistoryId(string fileHistoryId)
        {
            return dal.GetProjectIdByFileHistoryId(fileHistoryId);
        }

        /// <summary>
        /// 根据proejctId和category获取orderSort
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public int GetOrderSort(string projectId, string category)
        {
            return dal.GetOrderSort(projectId, category);
        }

        /// <summary>
        /// 根据 parentId 获取序号
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public int GetOrderSortForChildTab(string parentId)
        {
            return dal.GetOrderSortForChildTab(parentId);
        }
        
        public DataSet GetExpireDateByProjectId(string projectId)
        {
            return dal.GetExpireDateByProjectId(projectId);
        }

        public static bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion  ExtensionMethod
    }
}
