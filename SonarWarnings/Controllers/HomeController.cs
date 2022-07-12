using System.Web.Mvc;

namespace SonarWarnings.Controllers
{
    public class HomeController : Controller
    {
        #region Menu Action Methods
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View("About", (object)ConfigReader.DocumentationLink);
        }

        public ActionResult Contact()
        {
            return View();
        } 
        #endregion
    }
}
