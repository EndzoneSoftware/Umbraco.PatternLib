using System.Web.Mvc;
using System.Configuration;

namespace Endzone.Umbraco.PatternLib.Core
{
    public class PatternViewerEnabledAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var fooCookie = filterContext.HttpContext.Request.Cookies["foo"];

            if (ConfigurationManager.AppSettings["PatternLib.Enable"] != "true")
            {
                filterContext.Result = new HttpNotFoundResult("Disabled");
            }
        }
    }
}