using System.Collections.Generic;

namespace SonarWarnings
{
    public class ProjectStatus
    {
        public string Status { get; set; }
        public string LastAnalysisDate { get; set; }
        public List<Conditions> Ratings { get; set; }
        public List<Facets> Facets { get; set; }
    }
}