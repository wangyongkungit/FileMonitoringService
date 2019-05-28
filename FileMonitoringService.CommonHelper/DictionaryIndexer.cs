using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitoringService.CommonHelper
{
    public class DictionaryIndexer
    {
        private Dictionary<string, string> no = new Dictionary<string, string>();

        public string this[string index]
        {
            get
            {
                return no[index] != null ? no[index].ToString() : string.Empty; 
            }
            set
            {
                if (no.ContainsKey(index))
                {
                    no.Remove(index);
                }
                no.Add(index, value ?? string.Empty);
            }
        }
    }
}
