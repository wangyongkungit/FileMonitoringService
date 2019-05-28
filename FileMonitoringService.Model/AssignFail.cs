using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    public class AssignFail
    {
        public string projectID { get; set; }
        public string failMessage { get; set; }
        public int failCount { get; set; }
    }
}
