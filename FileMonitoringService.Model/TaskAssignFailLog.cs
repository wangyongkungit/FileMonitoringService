using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// TaskAssignFailLog:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class TaskAssignFailLog
    {
        public TaskAssignFailLog()
        { }
        #region Model
        private string _id;
        private string _projectid;
        private int? _failcount = 0;
        private int? _isremind;
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
        public int? FAILCOUNT
        {
            set { _failcount = value; }
            get { return _failcount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ISREMIND
        {
            set { _isremind = value; }
            get { return _isremind; }
        }
        #endregion Model

    }
}
