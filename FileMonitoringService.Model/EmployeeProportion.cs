using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    [Serializable]
    public partial class EmployeeProportion
    {
        public EmployeeProportion()
        { }
        #region Model
        private string _id;
        private string _employeeid;
        private decimal? _proportion;
        private string _parentemployeeid;
        private bool _isbranchleader = false;
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
        public string EMPLOYEEID
        {
            set { _employeeid = value; }
            get { return _employeeid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? PROPORTION
        {
            set { _proportion = value; }
            get { return _proportion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PARENTEMPLOYEEID
        {
            set { _parentemployeeid = value; }
            get { return _parentemployeeid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ISBRANCHLEADER
        {
            set { _isbranchleader = value; }
            get { return _isbranchleader; }
        }
        #endregion Model

    }
}
