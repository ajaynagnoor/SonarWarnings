using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using SonarQube.BusinessLogic;

namespace SonarWarnings.Controllers
{
    public class ActivityController : Controller
    {
        readonly ActivityBL activity = new ActivityBL();

        [AuthorizeUser]
        public ActionResult ProjectActivity()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SyncToDatabase(string projectKey, string projectName)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);

            List<Tuple<string, int>> warningsCount = WarningsProcessor.GetSonarWarningsCountsSinceLeakPeriod(ConfigReader.IssuesSinceLeakPeriod.Replace("sinceLeakPeriod=true", string.Empty).Replace("{0}", projectKey), username, password);
            List<Tuple<string, int>> warningsCountAsonAnalysisDate = WarningsProcessor.GetSonarWarningsCountsSinceLeakPeriod(ConfigReader.IssuesSinceLeakPeriod.Replace("{0}", projectKey), username, password);
            string response = SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.MetricsAPI.Replace("{0}", projectKey), username, password);
            string componentInfo = SonarQubeAuthentication.GetResponseFromRequest(string.Concat(ConfigReader.ProjectStatisticsAPI, "&projects=", projectKey), username, password);

            bool dbInsertStatus = activity.AddStatistics(warningsCount, warningsCountAsonAnalysisDate, response, componentInfo, projectName);
            return Json(dbInsertStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProjects()
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(ProjectsReader.GetProjects(ConfigReader.ProjectStatisticsAPI, username, password), JsonRequestBehavior.AllowGet);
        }
    }
}