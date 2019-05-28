using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model.WebApi
{
    public class Schedule
    {
        public string Check_type { get; set; }
        public string Class_id { get; set; }
        public string Class_setting_id { get; set; }
        public string Group_id { get; set; }
        public DateTime Plan_check_time { get; set; }
        public string Plan_id { get; set; }
        public string Userid { get; set; }
    }
}
