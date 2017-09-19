using System.Web.Mvc;
using System.Configuration;

namespace Endzone.Umbraco.PatternLib.Core.Attributes
{
    /// <summary>
    /// Only allow access to PatternLib when enabled in web.config.
    /// </summary>
    internal class PatternLibEnabledAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (ConfigurationManager.AppSettings["PatternLib.Enable"] != "true")
            {
                filterContext.Result = new HttpNotFoundResult("Disabled");
            }
        }
    }
}