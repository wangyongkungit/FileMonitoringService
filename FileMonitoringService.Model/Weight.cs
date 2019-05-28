using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMonitoringService.Model
{
    public class Weight
    {
        private decimal objectiveRightdown = 1M;

        public string EmployeeID { get; set; }

        /// <summary>
        /// 任务所需小时数
        /// </summary>
        public int TaskNeedHours { get; set; }

        /// <summary>
        /// 员工可支配时间
        /// </summary>
        public double EmpDisposableHours { get; set; }

        /// <summary>
        /// 时间空闲权重
        /// </summary>
        public decimal FreeTimeRight { get; set; }

        public RepeatCustomer RepeatCustomer { get; set; }

        /// <summary>
        /// 预期目标是否完成
        /// </summary>
        public bool ObjectiveIsFinished { get; set; }

        /// <summary>
        /// 预期目标完成超出比例
        /// </summary>
        public decimal ObjectiveFinishedProportion { get; set; }

        /// <summary>
        /// 预期目标完成后的降权比例
        /// </summary>
        public decimal ObjectiveRightdown
        {
            set { objectiveRightdown = value; } 
            get { return objectiveRightdown; }
        }

        /// <summary>
        /// 专业质量分（平均分）
        /// </summary>
        public decimal QualityScore { get; set; }

        /// <summary>
        /// 质量分权重
        /// </summary>
        public decimal QualityScoreRight { get; set; }

        public decimal FinalScore { get; set; }

        /// <summary>
        /// 任务所需专业
        /// </summary>
        public List<string> ProjectNeedSpc { get; set; }

        /// <summary>
        /// 员工掌握的专业
        /// </summary>
        public List<string> EmpCanDoSpc { get; set; }

        /// <summary>
        /// 工时倍数
        /// </summary>
        public int TimeMultiple { get; set; }
    }

    public class RepeatCustomer
    {
        public bool IsRepeat { get; set; }
        public decimal Proportion { get; set; }
    }

    public class Objective
    {
        public bool HasFinished { get; set; }
        public decimal ExceedProportion { get; set; }
    }
}
