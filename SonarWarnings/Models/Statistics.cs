using System;

namespace SonarWarnings
{
    public class Statistics
    {
        private string lastAnalysisDate = string.Empty;

        public string Key { get; set; }
        public string Name { get; set; }
        public string LastAnalysisDate
        {
            get
            {
                return !string.IsNullOrWhiteSpace(lastAnalysisDate) ? Convert.ToDateTime(lastAnalysisDate).ToShortDateString() : string.Empty;
            }
            set
            {
                lastAnalysisDate = value;
            }
        }
        public string QualityGateStatus { get; set; }
    }
}