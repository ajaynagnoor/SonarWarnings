using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SonarWarnings.Controllers
{
    public class CommonController : Controller
    {
        [HttpGet]
        public JsonResult Favourites()
        {
            string username = Convert.ToString(Session["Username"], CultureInfo.InvariantCulture);
            string password = Convert.ToString(Session["Password"], CultureInfo.InvariantCulture);
            return Json(SonarQubeAuthentication.GetResponseFromRequest(ConfigReader.ProjectsAPI, username, password), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportToExcel(object source, string fileName)
        {
            var grdReport = new GridView
            {
                DataSource = source
            };
            grdReport.DataBind();
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grdReport.RenderControl(htw);
            byte[] bindata = System.Text.Encoding.ASCII.GetBytes(sw.ToString());
            return File(bindata, "application/ms-excel", string.Concat(fileName, ".xls"));
        }
    }
}