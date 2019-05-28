using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model.WebApi
{
    public class Leave
    {
        public string userId { get; set; }
        public DateTime fromDatetime { get; set; }
        public DateTime toDatetime { get; set; }
        public int totalHours { get; set; }
    }
}
