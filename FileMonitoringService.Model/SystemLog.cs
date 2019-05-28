using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// systemlog:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class SystemLog
    {
        public SystemLog()
        { }
        #region Model
        private string _id;
        private string _operatetype;
        private string _operatecontent;
        private DateTime? _createdate;
        private string _employeeid;
        private string _ipaddress;
        private string _physicaladdress;
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
        public string OPERATETYPE
        {
            set { _operatetype = value; }
            get { return _operatetype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OPERATECONTENT
        {
            set { _operatecontent = value; }
            get { return _operatecontent; }
        }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>
        public DateTime? CREATEDATE
        {
            set { _createdate = value; }
            get { return _createdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EMPLOYEEID
        {
            set { _employeeid = value; }
            get { return _employeeid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string IPADDRESS
        {
            set { _ipaddress = value; }
            get { return _ipaddress; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PHYSICALADDRESS
        {
            set { _physicaladdress = value; }
            get { return _physicaladdress; }
        }
        #endregion Model

    }
}
