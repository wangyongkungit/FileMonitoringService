using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// EmployeeAccount:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class EmployeeAccount
    {
        public EmployeeAccount()
        { }
        #region Model
        private string _id;
        private decimal? _amount = 0.00M;
        private decimal? _paidamount = 0.00M;
        private decimal? _surplusamount = 0.00M;
        private decimal? _othersamount = 0.00M;
        private DateTime? _createdate;
        private string _employeeid;
        private DateTime? _lastupdatedate;
        private bool _isdeleted = false;
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
        public decimal? AMOUNT
        {
            set { _amount = value; }
            get { return _amount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? PAIDAMOUNT
        {
            set { _paidamount = value; }
            get { return _paidamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? SURPLUSAMOUNT
        {
            set { _surplusamount = value; }
            get { return _surplusamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? OTHERSAMOUNT
        {
            set { _othersamount = value; }
            get { return _othersamount; }
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
        /// on update CURRENT_TIMESTAMP
        /// </summary>
        public DateTime? LASTUPDATEDATE
        {
            set { _lastupdatedate = value; }
            get { return _lastupdatedate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ISDELETED
        {
            set { _isdeleted = value; }
            get { return _isdeleted; }
        }
        #endregion Model

    }
}
