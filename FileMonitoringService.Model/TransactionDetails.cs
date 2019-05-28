using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// TransactionDetails:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class TransactionDetails
    {
        public TransactionDetails()
        { }
        #region Model
        private string _id;
        private decimal? _transactionamount = 0.00M;
        private string _transactiondescription;
        private decimal? _transactionproportion;
        private DateTime? _transactiondate;
        private DateTime? _plandate;
        private int? _transactiontype;
        private string _employeeid;
        private string _projectid;
        private DateTime? _createdate;
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
        public decimal? TRANSACTIONAMOUNT
        {
            set { _transactionamount = value; }
            get { return _transactionamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TRANSACTIONDESCRIPTION
        {
            set { _transactiondescription = value; }
            get { return _transactiondescription; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? TRANSACTIONPROPORTION
        {
            set { _transactionproportion = value; }
            get { return _transactionproportion; }
        }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>
        public DateTime? TRANSACTIONDATE
        {
            set { _transactiondate = value; }
            get { return _transactiondate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PLANDATE
        {
            set { _plandate = value; }
            get { return _plandate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? TRANSACTIONTYPE
        {
            set { _transactiontype = value; }
            get { return _transactiontype; }
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
        public string PROJECTID
        {
            set { _projectid = value; }
            get { return _projectid; }
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
        public bool ISDELETED
        {
            set { _isdeleted = value; }
            get { return _isdeleted; }
        }
        #endregion Model

    }
}
