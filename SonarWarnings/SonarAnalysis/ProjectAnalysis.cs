using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace SonarWarnings
{
    public static class ProjectAnalysis
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static ProjectStatus GetAnalysis(string analysisResponse, string issuesResponse) ////, string analysisDateResponse)
        {
            List<JToken> tokens = ((JObject)JsonConvert.DeserializeObject(analysisResponse)).Children().ToList()[0].ToList()[0].Children().ToList();
            List<Conditions> conditions = new List<Conditions>();

            List<JToken> analysisTokens = tokens[1].Children().Children().ToList();

            foreach (JToken token in analysisTokens)
            {
                conditions.Add(JsonConvert.DeserializeObject<Conditions>(Convert.ToString(token)));
            }

            string status = Convert.ToString(((JValue)((JProperty)tokens[0]).Value).Value);

            return new ProjectStatus
            {
                Status = status == "ERROR" ? "Failed" : status == "NONE" ? "Not Analyzed" : "Passed",
                Ratings = conditions,
                Facets = GetFacets(issuesResponse),
                ////LastAnalysisDate = GetLastAnalysisDate(analysisDateResponse)
            };
        }

        private static List<Facets> GetFacets(string resp)
        {
            JObject allFacets = (JObject)JsonConvert.DeserializeObject(resp);
            return JsonConvert.DeserializeObject<List<Facets>>(Convert.ToString(((JProperty)allFacets.Children().ToList()[8].ToList()[0].Children().Children().ToList()[1]).Value));
        }

        private static string GetLastAnalysisDate(string response)
        {
            string lastAnalysisDate = "NA";
            JObject allIssues = (JObject)JsonConvert.DeserializeObject(response);
            List<JToken> tokens = allIssues.Children().ToList()[0].ToList()[0].Children().ToList();
            List<Activity> activities = new List<Activity>();

            foreach (JToken token in tokens)
            {
                activities.Add(JsonConvert.DeserializeObject<Activity>(Convert.ToString(token, CultureInfo.InvariantCulture)));
            }

            if (activities != null && activities.Any())
            {
                var activityDates = (from res in activities
                                     select res.SubmittedAt.Date).Distinct();

                lastAnalysisDate = activityDates.Count() == 1 ? activityDates.FirstOrDefault().ToShortDateString()
                                            : activityDates.OrderByDescending(p => p).Skip(1).Take(1).FirstOrDefault().ToShortDateString();
            }

            logger.Trace(lastAnalysisDate, CultureInfo.InvariantCulture);
            return lastAnalysisDate;
        }
    }
}