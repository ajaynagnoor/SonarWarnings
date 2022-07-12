using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace SonarWarnings.Controllers
{
    public class DashboardController : Controller
    {
        [AuthorizeUser]
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Analysis(string projectKey)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            string analysisResponse = SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectsAnalysisAPI.Replace("{0}", projectKey), username, password);
            string issuesResponse = SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.FacetsAPI.Replace("{0}", projectKey), username, password);
            ProjectStatus status = ProjectAnalysis.GetAnalysis(analysisResponse, issuesResponse);
            status.LastAnalysisDate = GetLastAnalysisDateByProjectKey(projectKey);

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        private string GetLastAnalysisDateByProjectKey(string projectKey)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            string apiUrl = string.Concat(ConfigReader.ProjectStatisticsAPI, "&projects=" + projectKey);
            List<Statistics> statistics = ProjectSummary.GetStatistics(SonarQubeAuthentication.GetResponseFromRequest(apiUrl, username, password));

            if (!string.IsNullOrWhiteSpace(statistics[0].LastAnalysisDate))
            {
                return Convert.ToDateTime(statistics[0].LastAnalysisDate).ToShortDateString();
            }
            return "NA";
        }
    }
}