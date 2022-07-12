using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace SonarWarnings.Controllers
{
    public class StatisticsController : CommonController
    {
        [HttpGet]
        [AuthorizeUser]
        public ActionResult ProjectStatistics()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetProjectStatistics()
        {
            return Json(GetStatistics(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportStatistis()
        {
            ////return RedirectToAction("ExportToExcel", "Common", new { source = GetStatistics(), fileName = "Statistics" });
            return ExportToExcel(GetStatistics(), "Statistics");
        }

        [HttpGet]
        public ActionResult SonarWarningsCountsSinceLeakPeriod(string projectKey)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(WarningsProcessor.GetSonarWarningsCountsSinceLeakPeriod(ConfigReader.IssuesSinceLeakPeriod.Replace("{0}", projectKey), username, password), JsonRequestBehavior.AllowGet);
        }

        private List<Statistics> GetStatistics()
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            string response = SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectStatisticsAPI, username, password);
            return ProjectSummary.GetQualityGateStatus(ProjectSummary.GetStatistics(response), username, password);
        }
    }
}