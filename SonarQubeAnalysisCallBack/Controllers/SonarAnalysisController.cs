using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SonarQube.BusinessLogic;

namespace SonarQubeAnalysisCallBack.Controllers
{
    public class SonarAnalysisController : ApiController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public OkResult Get()
        {
            logger.Info("Get Method called");
            return Ok();
        }

        [HttpPost]
        public OkResult SonarProjectStatusCallBackAsync([FromBody] SonarQubeVm sonar)
        {
            ActivityBL activity = new ActivityBL();

            logger.Info("Post Method called");

            try
            {
                Task.Run(() => ProcessCallBackResult(sonar, activity)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return Ok();
        }

        private static void ProcessCallBackResult(SonarQubeVm sonar, ActivityBL activity)
        {
            logger.Info("Key - " + sonar.Project.Key);
            logger.Info("Name - " + sonar.Project.Name);

            string SonarIssueApiLink = ConfigurationManager.AppSettings["IssuesSinceLeakPeriod"];
            string MetricsAPI = ConfigurationManager.AppSettings["MetricsApi"];
            string ProjectStatisticsAPI = ConfigurationManager.AppSettings["ProjectsStatisticsApi"];

            List<Tuple<string, int>> warningsCount = GetSonarWarningsCountsSinceLeakPeriod(SonarIssueApiLink.Replace("sinceLeakPeriod=true", string.Empty).Replace("{0}", sonar.Project.Key));
            List<Tuple<string, int>> warningsCountAsonAnalysisDate = GetSonarWarningsCountsSinceLeakPeriod(SonarIssueApiLink.Replace("{0}", sonar.Project.Key));
            string response = GetResponseFromRequest(MetricsAPI.Replace("{0}", sonar.Project.Key));
            string componentInfo = GetResponseFromRequest(string.Concat(ProjectStatisticsAPI, "&projects=", sonar.Project.Key));

            bool dbInsertStatus = activity.AddStatistics(warningsCount, warningsCountAsonAnalysisDate, response, componentInfo, sonar.Project.Name);
            logger.Info(string.Concat("Latest statistics inserted for project - ", sonar.Project.Name, (dbInsertStatus ? "Success" : "Failed")));
        }

        private static List<Tuple<string, int>> GetSonarWarningsCountsSinceLeakPeriod(string url)
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
                        int issueCount = GetIssuesCountSinceLeakPeriod(GetResponseFromRequest(url));
                        tuples.Add(Tuple.Create(string.Concat(issuetype, "-", severity), issueCount));
                        url = url.Replace(issuetype, "{1}").Replace(severity, "{2}");
                    }
                }
                else
                {
                    url = url.Replace("{1}", issuetype).Replace("&severities={2}", string.Empty);
                    int issueCount = GetIssuesCountSinceLeakPeriod(GetResponseFromRequest(url));
                    tuples.Add(Tuple.Create(issuetype, issueCount));
                }
            }
            return tuples;
        }

        private static int GetIssuesCountSinceLeakPeriod(string response)
        {
            return Convert.ToInt32(((JValue)((JProperty)((JObject)JsonConvert.DeserializeObject(response)).Children().First()).Value).Value);
        }

        private static string GetResponseFromRequest(string url)
        {
            string responseInText = string.Empty;

            try
            {
                string SonarAdminUser = ConfigurationManager.AppSettings["SonarAdminUser"];
                string SonarAdminPassword = ConfigurationManager.AppSettings["SonarAdminPassword"];

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "Get";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(string.Concat(SonarAdminUser, ":", SonarAdminPassword)));

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    responseInText = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                responseInText = ex.Message;
                logger.Error(ex);
            }

            return responseInText;
        }
    }

    public class SonarQubeVm
    {
        public DateTime? AnalysedAt { get; set; }
        public SonarProjectInfo Project { get; set; }
        public string ServerUrl { get; set; }
        public string Status { get; set; }
        public string TaskId { get; set; }
    }

    public class SonarProjectInfo
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }
}
