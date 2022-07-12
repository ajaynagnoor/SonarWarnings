using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace SonarWarnings.Controllers
{
    public class WarningsController : CommonController
    {
        public ActionResult Warnings()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SonarWarnings(string projectName, string issueType, string severity, string newcodewarnings)
        {
            ////string OnlyAlphaExp = @"^[a-zA-Z]*$";
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(WarningsProcessor.GetResponse(projectName, issueType, severity, GetApiToCall(newcodewarnings), username, password).AsQueryable<SonarWarning>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportWarnings(string projectName, string issueType, string severity, string newcodewarnings)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return ExportToExcel(WarningsProcessor.GetResponse(projectName, issueType, severity, GetApiToCall(newcodewarnings), username, password), projectName);
        }

        private string GetApiToCall(string newcodewarnings)
        {
            return newcodewarnings.Equals("true") ? ConfigReader.IssuesSinceLeakPeriod : ConfigReader.IssuesAPI;
        }
    }
}