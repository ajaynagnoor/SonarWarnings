using System;
using System.Globalization;
using System.Web.Mvc;

namespace SonarWarnings.Controllers
{
    public class QualityProfilesController : CommonController
    {
        [HttpGet]
        public ActionResult QualityProfiles()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetQualityProfilesForSelectedLanguage(string qplanguage)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.QualityProfilesAPI.Replace("{0}", qplanguage), username, password), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QualityProfileRules(string profileKey, string language)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(QualityProfileReader.GetResponse(ConfigReader.RulesAPI.Replace("{0}", profileKey).Replace("{1}", language), username, password), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportRules(string profileKey, string language)
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return ExportToExcel(QualityProfileReader.GetResponse(ConfigReader.RulesAPI.Replace("{0}", profileKey).Replace("{1}", language), username, password), language);
        }

        [HttpGet]
        public ActionResult GetLanguages()
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.LanguageAPI, username, password), JsonRequestBehavior.AllowGet);
        }
    }
}