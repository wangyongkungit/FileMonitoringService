using FileMonitoringDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringServiceBLL
{
    public class ProjectSpecialtyBLL
    {
        ProjectSpecialtyDAL dal=new ProjectSpecialtyDAL();
        /// <summary>
        /// 根据任务ID获取任务专业
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="type">专业大类类型</param>
        /// <returns></returns>
        public DataSet GetSpecialtyInnerJoinProject(string projectID, string type)
        {
           return  dal.GetSpecialtyInnerJoinProject(projectID, type);
        }
    }
}
