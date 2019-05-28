﻿using FileMonitoringDAL;
using FileMonitoringService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringServiceBLL
{
    public partial class SystemLogBLL
    {
        private readonly SystemLogDAL dal = new SystemLogDAL();
        public SystemLogBLL()
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
        public bool Add(SystemLog model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(SystemLog model)
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
        /*/// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string IDlist )
        {
            return dal.DeleteList(Maticsoft.Common.PageValidate.SafeLongFilter(IDlist,0) );
        }*/

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public SystemLog GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /*/// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Maticsoftsystemlog GetModelByCache(string ID)
        {
			
            string CacheKey = "systemlogModel-" + ID;
            object objModel = Maticsoft.Common.DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(ID);
                    if (objModel != null)
                    {
                        int ModelCache = Maticsoft.Common.ConfigHelper.GetConfigInt("ModelCache");
                        Maticsoft.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch{}
            }
            return (Maticsoftsystemlog)objModel;
        }*/

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
        public List<SystemLog> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<SystemLog> DataTableToList(DataTable dt)
        {
            List<SystemLog> modelList = new List<SystemLog>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                SystemLog model;
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
        #endregion  BasicMethod
    }
}
