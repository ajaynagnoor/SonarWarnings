using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace SonarWarnings
{
    public static class WarningsProcessor
    {
        private const int MAX_SONARQUBE_API_LIMIT = 10000;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        internal static List<SonarWarning> GetResponse(string projectName, string issueType, string severity, string url, string UserName, string Password)
        {
            StringBuilder pageCount = new StringBuilder();
            List<SonarWarning> warnings = new List<SonarWarning>();

            try
            {
                string response = url.Contains("localhost:9000") ? SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectStatisticsAPI, UserName, Password)
                    : SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectStatisticsAPI);

                string projectKey = (from result in ProjectSummary.GetStatistics(response)
                                     where result.Name == projectName
                                     select result.Key).FirstOrDefault();

                url = severity.Equals("-2") ? url.Replace("{0}", projectKey).Replace("{1}", issueType).Replace("&severities={2}", string.Empty)
                                            : url.Replace("{0}", projectKey).Replace("{1}", issueType).Replace("{2}", severity);

                logger.Trace(url, CultureInfo.InvariantCulture);

                int issueCount = GetIssuesCount(SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password));

                int iterations = (issueCount % 500) == 0 ? (issueCount / 500) : (issueCount / 500) + 1;

                for (int iteration = 1; iteration <= iterations; iteration++)
                {
                    pageCount.Append("&p=").Append(iteration.ToString());
                    warnings.AddRange(GetWarnings(SonarQubeAuthentication.GetResponseFromRequest(string.Concat(url, pageCount.ToString()), UserName, Password)));
                    pageCount.Clear();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            foreach (var warning in warnings)
            {
                warning.Rule = warning.Rule.Split(':')[1];
                warning.Component = (warning.Component.Split(':').Length > 1) ? warning.Component.Split(':')[1] : warning.Component;
                warning.CreationDate = Convert.ToDateTime(warning.CreationDate, CultureInfo.InvariantCulture).ToShortDateString();
                warning.ClosedDate = !string.IsNullOrWhiteSpace(warning.ClosedDate) ? Convert.ToDateTime(warning.ClosedDate, CultureInfo.InvariantCulture).ToShortDateString() : string.Empty;
            }

            return warnings;
        }

        internal static List<SonarWarning> GetResponse1(string projectName, string issueType, string severity, string url, string UserName, string Password)
        {
            ////ReadActivity(UserName, Password);

            StringBuilder pageCount = new StringBuilder();
            List<SonarWarning> warnings = new List<SonarWarning>();

            try
            {
                string projectKey = (from result in ProjectSummary.GetStatistics(SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectStatisticsAPI, UserName, Password))
                                     where result.Name == projectName
                                     select result.Key).FirstOrDefault();

                url = severity.Equals("-1") ? url.Replace("{0}", projectKey).Replace("{1}", issueType).Replace("&severities={2}", string.Empty)
                                            : url.Replace("{0}", projectKey).Replace("{1}", issueType).Replace("{2}", severity);

                logger.Trace(url);

                int issueCount = GetIssuesCount(SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password));

                int iterations = (issueCount % 500) == 0 ? (issueCount / 500) : (issueCount / 500) + 1;

                for (int iteration = 1; iteration <= iterations; iteration++)
                {
                    pageCount.Append("&p=").Append(iteration.ToString());
                    warnings.AddRange(GetWarnings(SonarQubeAuthentication.GetResponseFromRequest(string.Concat(url, pageCount.ToString()), UserName, Password)));
                    pageCount.Clear();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            foreach (var warning in warnings)
            {
                warning.Rule = warning.Rule.Split(':')[1];
                warning.Component = (warning.Component.Split(':').Length > 1) ? warning.Component.Split(':')[1] : warning.Component;
            }

            return warnings;
        }

        private static void ReadActivity(string UserName, string Password)
        {
            string response = SonarQubeAuthentication.GetResponseFromRequest("http://localhost:9000/api/ce/activity?component=SonarWarnings", UserName, Password);

            JObject allIssues = (JObject)JsonConvert.DeserializeObject(response);
            List<JToken> tokens = allIssues.Children().ToList()[0].ToList()[0].Children().ToList();
            List<Activity> activities = new List<Activity>();
            foreach (JToken token in tokens)
            {
                activities.Add(JsonConvert.DeserializeObject<Activity>(Convert.ToString(token)));
            }

            var result = (from res in activities
                          select res.SubmittedAt.Date).Distinct().OrderByDescending(p => p).Skip(1).Take(1).FirstOrDefault().ToShortDateString();

        }

        internal static List<Tuple<string, int>> GetSonarWarningsCountsSinceLeakPeriod(string url, string UserName, string Password)
        {
            List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();
            string[] issueTypes = { "CODE_SMELL", "BUG", "VULNERABILITY", "SECURITY_HOTSPOT" };
            string[] severities = { "BLOCKER", "CRITICAL", "MAJOR", "MINOR", "INFO" };

            foreach (var issuetype in issueTypes)
            {
                if (issuetype != "SECURITY_HOTSPOT")
                {
                    foreach (var severity in severities)
                    {
                        url = url.Replace("{1}", issuetype).Replace("{2}", severity);
                        int issueCount = GetIssuesCountSinceLeakPeriod(SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password));
                        tuples.Add(Tuple.Create(string.Concat(issuetype, "-", severity), issueCount));
                        url = url.Replace(issuetype, "{1}").Replace(severity, "{2}");
                    }
                }
                else
                {
                    url = url.Replace("{1}", issuetype).Replace("&severities={2}", string.Empty);
                    int issueCount = GetIssuesCountSinceLeakPeriod(SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password));
                    tuples.Add(Tuple.Create(issuetype, issueCount));
                }
            }
            return tuples;
        }

        private static List<SonarWarning> GetWarnings(string response)
        {
            JObject allIssues = (JObject)JsonConvert.DeserializeObject(response);
            List<JToken> tokens = allIssues.Children().ToList()[6].ToList()[0].Children().ToList();
            List<SonarWarning> warnings = new List<SonarWarning>();

            foreach (JToken token in tokens)
            {
                warnings.Add(JsonConvert.DeserializeObject<SonarWarning>(Convert.ToString(token)));
            }

            return warnings;
        }

        private static int GetIssuesCount(string response)
        {
            int count = Convert.ToInt32(((JValue)((JProperty)((JObject)JsonConvert.DeserializeObject(response)).Children().First()).Value).Value);

            ////This is the maximum limit of the SonarQube API
            if (count > MAX_SONARQUBE_API_LIMIT)
            {
                count = MAX_SONARQUBE_API_LIMIT;
            }

            return count;
        }

        private static int GetIssuesCountSinceLeakPeriod(string response)
        {
            return Convert.ToInt32(((JValue)((JProperty)((JObject)JsonConvert.DeserializeObject(response)).Children().First()).Value).Value);
        }
    }
}