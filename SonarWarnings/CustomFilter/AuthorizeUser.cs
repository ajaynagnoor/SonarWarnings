using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SonarWarnings
{
    public class AuthorizeUser : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.SkipAuthorization)
            {
                return httpContext.SkipAuthorization;
            }
            string username = Convert.ToString(httpContext.Session["Username"], CultureInfo.InvariantCulture);
            httpContext.SkipAuthorization = Convert.ToString(ConfigurationManager.AppSettings["AdminUsers"], CultureInfo.InvariantCulture).Contains(username);
            return httpContext.SkipAuthorization;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Login" },
                    { "action", "UnAuthorized" }
               });
        }
    }
}