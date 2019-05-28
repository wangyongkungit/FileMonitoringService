using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringDAL
{
    public class ProjectSpecialtyDAL
    {
        /// <summary>
        /// 根据任务ID获取任务专业
        /// </summary>
        /// <param name="projectID">任务ID</param>
        /// <param name="type">专业大类类型</param>
        /// <returns></returns>
        public DataSet GetSpecialtyInnerJoinProject(string projectID, string type)
        {
            type = "0";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT p.id pid, ps.id psid, ps.specialtyid, ps.type, cv.CONFIGVALUE SPECIALTYNAME from project p
                             LEFT JOIN projectspecialty ps
                             on p.id = ps.projectid
                            LEFT JOIN configvalue cv
                            ON ps.SPECIALTYID = cv.CONFIGKEY
                            AND CONFIGTYPEID = 'b47d2587-6421-4dc5-b0be-7ce595d6bdc0'
                             where p.id = '" + projectID + "'");
            if (!string.IsNullOrEmpty(type))
            {
                strSql.Append(" and ps.type = '" + type + "'");
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }
    }
}
