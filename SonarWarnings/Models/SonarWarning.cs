namespace SonarWarnings
{
    public class SonarWarning
    {
        public string Rule { get; set; }
        public string Severity { get; set; }
        public string Component { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Effort { get; set; }
        public string Type { get; set; }
        public string Line { get; set; }
        public string Resolution { get; set; }
        public string CreationDate { get; set; }
        public string ClosedDate { get; set; }
    }
}