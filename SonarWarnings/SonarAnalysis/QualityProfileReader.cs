using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SonarWarnings
{
    public static class QualityProfileReader
    {
        private const int MAX_SONARQUBE_API_LIMIT = 10000;

        internal static List<QualityProfileRules> GetResponse(string url, string UserName, string Password)
        {
            List<QualityProfileRules> qualityProfileRules = new List<QualityProfileRules>();

            try
            {
                int issueCount = GetIssuesCount(SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password));

                int iterations = (issueCount % 500) == 0 ? (issueCount / 500) : (issueCount / 500) + 1;

                for (int iteration = 1; iteration <= iterations; iteration++)
                {
                    qualityProfileRules.AddRange(GetRules(SonarQubeAuthentication.GetResponseFromRequest(url, UserName, Password)));
                }
            }
            catch
            {
                ////MessageBox.Show(string.Concat(ex.Message, " Please try again after some time."), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (var rule in qualityProfileRules)
            {
                rule.Key = rule.Key.Split(':')[1];
            }

            return qualityProfileRules;
        }

        private static List<QualityProfileRules> GetRules(string response)
        {
            JObject allIssues = (JObject)JsonConvert.DeserializeObject(response);
            List<JToken> tokens = allIssues.Children().ToList()[3].ToList()[0].Children().ToList();
            List<QualityProfileRules> qualityProfileRules = new List<QualityProfileRules>();

            foreach (JToken token in tokens)
            {
                qualityProfileRules.Add(JsonConvert.DeserializeObject<QualityProfileRules>(Convert.ToString(token)));
            }

            return qualityProfileRules;
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
    }
}