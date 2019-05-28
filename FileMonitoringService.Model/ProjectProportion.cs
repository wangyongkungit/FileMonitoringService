using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// ProjectProportion:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class ProjectProportion
    {
        public ProjectProportion()
        { }
        #region Model
        private string _id;
        private string _projectid;
        private decimal? _proportion = 0.00M;
        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PROJECTID
        {
            set { _projectid = value; }
            get { return _projectid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? PROPORTION
        {
            set { _proportion = value; }
            get { return _proportion; }
        }
        #endregion Model

    }
}
