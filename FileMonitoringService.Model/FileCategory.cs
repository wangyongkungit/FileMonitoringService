using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// FileCategory:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class FileCategory
    {
        public FileCategory()
        { }
        #region Model
        private string _id;
        private string _projectid;
        private string _category;
        private string _title;
        private string _description;
        private string _foldername;
        private DateTime? _createdate;
        private string _createuser;
        private DateTime? _expiredate;
        private string _parentid;
        private int? _classsort;
        private int? _divisionsort;
        private int? _ordersort;
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
        public string CATEGORY
        {
            set { _category = value; }
            get { return _category; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TITLE
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DESCRIPTION
        {
            set { _description = value; }
            get { return _description; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FOLDERNAME
        {
            set { _foldername = value; }
            get { return _foldername; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CREATEDATE
        {
            set { _createdate = value; }
            get { return _createdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CREATEUSER
        {
            set { _createuser = value; }
            get { return _createuser; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EXPIREDATE
        {
            set { _expiredate = value; }
            get { return _expiredate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PARENTID
        {
            set { _parentid = value; }
            get { return _parentid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? CLASSSORT
        {
            set { _classsort = value; }
            get { return _classsort; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? DIVISIONSORT
        {
            set { _divisionsort = value; }
            get { return _divisionsort; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ORDERSORT
        {
            set { _ordersort = value; }
            get { return _ordersort; }
        }
        #endregion Model

    }
}
