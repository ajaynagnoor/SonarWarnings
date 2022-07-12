using System;
using System.Configuration;

namespace SonarWarnings
{
    public static class ConfigReader
    {
        public static string IssuesAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["IssuesApi"];
            }
        }

        public static string AllUsersAPI
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["UsersApi"]);
            }
        }

        public static string ProjectsAPI
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ProjectsApi"]);
            }
        }

        public static string ProjectsAnalysisAPI
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ProjectsAnalysisApi"]);
            }
        }

        public static string ProjectStatisticsAPI
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ProjectsStatisticsApi"]);
            }
        }

        public static string FacetsAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["FacetsAPI"];
            }
        }

        public static string DocumentationLink
        {
            get
            {
                return ConfigurationManager.AppSettings["DocumentationLink"];
            }
        }

        public static string IssuesSinceLeakPeriod
        {
            get
            {
                return ConfigurationManager.AppSettings["IssuesSinceLeakPeriod"];
            }
        }

        public static string RulesAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["RulesAPI"];
            }
        }

        public static string QualityProfilesAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["QualityProfilesAPI"];
            }
        }
        public static string ActivityAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["ActivityAPI"];
            }
        }
        public static string ComponentAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["ComponentAPI"];
            }
        }
        public static string LanguageAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["LanguageAPI"];
            }
        }
        public static string MetricsAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["MetricsAPI"];
            }
        }
        public static string ComponentInfoAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["ComponentInfoAPI"];
            }
        }
    }
}