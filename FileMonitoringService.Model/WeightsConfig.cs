using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    /// <summary>
    /// WeightsConfig:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class WeightsConfig
    {
        public WeightsConfig()
        { }
        #region Model
        private string _id;
        private string _itemkey;
        private string _itemname;
        private decimal? _itemvalue;
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
        public string ITEMKEY
        {
            set { _itemkey = value; }
            get { return _itemkey; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ITEMNAME
        {
            set { _itemname = value; }
            get { return _itemname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? ITEMVALUE
        {
            set { _itemvalue = value; }
            get { return _itemvalue; }
        }
        #endregion Model

    }
}
