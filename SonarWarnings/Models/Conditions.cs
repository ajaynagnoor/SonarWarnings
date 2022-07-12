namespace SonarWarnings
{
    public class Conditions
    {
        public string Status { get; set; }
        public string MetricKey { get; set; }
        public string Comparator { get; set; }
        public string PeriodIndex { get; set; }
        public string ErrorThreshold { get; set; }
        public string ActualValue { get; set; }
    }
}