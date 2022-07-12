using System;
using System.Web.Mvc;

namespace SonarWarnings.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(string username, string password)
        {
            Tuple<bool, string> tuple = SonarQubeAuthentication.ValidateUser(ConfigReader.AllUsersAPI, username, password);

            if (tuple.Item1)
            {
                Session["Username"] = username;
                Session["Password"] = password;
            }

            return Json(tuple, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("About", "Home");
        }

        [HttpGet]
        public ActionResult UnAuthorized()
        {
            ViewBag.Message = "UnAuthorized Page!";
            return View();
        }
    }
}