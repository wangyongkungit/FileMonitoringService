using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    public class Employee
    {
        public Employee()
        { }
        #region Model
        private string _id;
        private string _employeeno;
        private string _password;
        private string _name;
        private bool _sex;
        private DateTime? _birthdate;
        private string _nativeplace;
        private string _mobilephone;
        private string _address;
        private string _email;
        private string _bankcard;
        private string _departmentid;
        private string _politicalstatus;
        private decimal? _type;
        private decimal? _available;
        private string _dingtalkuserid;
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 员工编号
        /// </summary>
        public string EMPLOYEENO
        {
            set { _employeeno = value; }
            get { return _employeeno; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PASSWORD
        {
            set { _password = value; }
            get { return _password; }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string NAME
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public bool SEX
        {
            set { _sex = value; }
            get { return _sex; }
        }
        /// <summary>
        /// 出生年月
        /// </summary>
        public DateTime? BIRTHDATE
        {
            set { _birthdate = value; }
            get { return _birthdate; }
        }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string NATIVEPLACE
        {
            set { _nativeplace = value; }
            get { return _nativeplace; }
        }
        /// <summary>
        /// 手机
        /// </summary>
        public string MOBILEPHONE
        {
            set { _mobilephone = value; }
            get { return _mobilephone; }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDRESS
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string EMAIL
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BANKCARD
        {
            set { _bankcard = value; }
            get { return _bankcard; }
        }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DEPARTMENTID
        {
            set { _departmentid = value; }
            get { return _departmentid; }
        }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string POLITICALSTATUS
        {
            set { _politicalstatus = value; }
            get { return _politicalstatus; }
        }
        /// <summary>
        /// 员工类型
        /// </summary>
        public decimal? TYPE
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public decimal? AVAILABLE
        {
            set { _available = value; }
            get { return _available; }
        }
        /// <summary>
        /// 钉钉UserId
        /// </summary>
        public string DINGTALKUSERID
        {
            set { _dingtalkuserid = value; }
            get { return _dingtalkuserid; }
        }
        #endregion Model
    }
}
