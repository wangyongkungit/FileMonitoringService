using FileMonitoringDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringServiceBLL
{
    public class TaskAssignConfigBLL
    {
        TaskAssignConfigDAL tacDal = new TaskAssignConfigDAL();
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return tacDal.GetList(strWhere);
        }
    }
}
