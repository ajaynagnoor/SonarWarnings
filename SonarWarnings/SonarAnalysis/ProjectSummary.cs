using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace SonarWarnings
{
    public static class ProjectSummary
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        internal static List<Statistics> GetStatistics(string response)
        {
            List<Statistics> statistics = new List<Statistics>();

            try
            {
                List<JToken> tokens = ((JObject)JsonConvert.DeserializeObject(response)).Children().ToList()[1].ToList()[0].Children().ToList();

                foreach (JToken token in tokens)
                {
                    statistics.Add(JsonConvert.DeserializeObject<Statistics>(Convert.ToString(token)));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return statistics;
        }

        internal static List<Statistics> GetQualityGateStatus(List<Statistics> statistics, string username, string password)
        {
            foreach (Statistics stat in statistics)
            {
                string analysisResponse = SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectsAnalysisAPI.Replace("{0}", stat.Key), username, password);
                List<JToken> tokens = ((JObject)JsonConvert.DeserializeObject(analysisResponse)).Children().ToList()[0].ToList()[0].Children().ToList();

                if (!string.IsNullOrWhiteSpace(stat.LastAnalysisDate))
                {
                    stat.QualityGateStatus = Convert.ToString(((JValue)((JProperty)tokens[0]).Value).Value).Equals("ERROR") ? "Failed" : "Passed";
                }
            }
            return statistics;
        }
    }
}