using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    public class RightDown
    {
        public RightDown()
        { }
        #region Model
        private string _id;
        private int? _fromvalue;
        private int? _tovalue;
        private decimal? _rightdownpercent;
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
        public int? FROMVALUE
        {
            set { _fromvalue = value; }
            get { return _fromvalue; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? TOVALUE
        {
            set { _tovalue = value; }
            get { return _tovalue; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? RIGHTDOWNPERCENT
        {
            set { _rightdownpercent = value; }
            get { return _rightdownpercent; }
        }
        #endregion Model
    }
}
